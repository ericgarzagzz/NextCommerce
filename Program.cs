using NextCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Data.Enums;
using NextCommerce.Services;
using NuGet.Packaging;
using System.Globalization;
using NextCommerce.Extensions;
using NextCommerce.Models.Configurations;
using Microsoft.Extensions.FileProviders;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
{
    builder.Services.AddHangfire(options =>
    {
        options.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
        options.UseSimpleAssemblyNameTypeSerializer();
        options.UseRecommendedSerializerSettings();
        options.UseInMemoryStorage();
    });

    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("NextCommerce"));
}
else
{
    builder.Services.AddHangfire(options =>
    {
        options.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
        options.UseSimpleAssemblyNameTypeSerializer();
        options.UseRecommendedSerializerSettings();
        options.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        });
    });

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddHangfireServer();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "NextCommerce";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services
    .AddAuthentication()
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHashIds(builder.Configuration.GetValue("HashingIdSalt", ""));

builder.Configuration.GetRequiredSection("Email").Bind(EmailSettings.Instance);

var fluentEmailBuilder = builder.Services.AddFluentEmail(EmailSettings.Instance.Address, EmailSettings.Instance.Name)
    .AddLiquidRenderer();

if (EmailSettings.Instance.Type == "SendGrid")
{
    if (EmailSettings.Instance.SendGrid == null) throw new Exception("Email settings are set to use SendGrid, but no SendGrid configurations were given.");

    fluentEmailBuilder = fluentEmailBuilder
        .AddSendGridSender(EmailSettings.Instance.SendGrid.Key, EmailSettings.Instance.SendGrid.Sandbox);
}
else if (EmailSettings.Instance.Type == "Smtp")
{
    if (EmailSettings.Instance.Smtp == null) throw new Exception("Email settings are set to use Smtp, but no Smtp configurations were given.");

    fluentEmailBuilder = fluentEmailBuilder
        .AddMailKitSender(new FluentEmail.MailKitSmtp.SmtpClientOptions
        {
            Server = EmailSettings.Instance.Smtp.Host,
            Port = EmailSettings.Instance.Smtp.Port,
            User = EmailSettings.Instance.Smtp.Username,
            Password = EmailSettings.Instance.Smtp.Password,
            UseSsl = EmailSettings.Instance.Smtp.Ssl
        });
}
else throw new NotImplementedException("Email settings are set to use an invalid or a not implemented type.");

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IServiceBannersService, ServiceBannersService>();
builder.Services.AddScoped<IProductShowcaseCollectionService, ProductShowcaseCollectionService>();
builder.Services.AddScoped<IBrandsService, BrandsService>();
builder.Services.AddScoped<ISlidersService, SlidersService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IShoppingSessionService, ShoppingSessionService>();
builder.Services.AddScoped<IStripeClientProviderService, StripeClientProviderService>();
builder.Services.AddScoped<IStripePaymentIntentService, StripePaymentIntentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailViewModelService, EmailViewModelService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

var app = builder.Build();


#region Send test email
using (var scope = app.Services.CreateScope())
{
    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        emailService.SendTestEmail("ericgarzaroot@gmail.com");
    } catch (Exception ex)
    {
        logger.LogCritical(ex, "Test email validation failed.");
    }
}
#endregion

    app.Use(async (context, next) =>
{
    CultureInfo.CurrentCulture = new CultureInfo("es-MX");
    CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    #region Seed database
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        context.Database.EnsureCreated();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var defaultAdminEmail = app.Configuration.GetValue<string>("DefaultAdminUser:Email");

        if (!userManager.Users.Any(u => u.Email == defaultAdminEmail))
        {
            var result = userManager.CreateAsync(new ApplicationUser
            {
                Email = defaultAdminEmail,
                UserName = app.Configuration.GetValue<string>("DefaultAdminUser:UserName"),
                EmailConfirmed = true
            }, app.Configuration.GetValue<string>("DefaultAdminUser:Password")).Result;

            if (!result.Succeeded)
            {
                logger.LogError("Default admin user cannot be created. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => $"{e.Code} => {e.Description}")));
                throw new Exception("Default admin user cannot be created.");
            }
        }

        var adminUser = userManager.Users.First(u => u.Email == defaultAdminEmail);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        if (!roleManager.RoleExistsAsync(Role.Admin.ToString()).Result)
        {
            var result = roleManager.CreateAsync(new ApplicationRole
            {
                Name = Role.Admin.ToString()
            }).Result;

            if (!result.Succeeded)
            {
                logger.LogError("Admin role could not be created. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => $"{e.Code} => {e.Description}")));
                throw new Exception("Admin role could not be created.");
            }
        }

        var adminUserIsInAdminRole = userManager.IsInRoleAsync(adminUser, Role.Admin.ToString()).Result;

        if (!adminUserIsInAdminRole)
        {
            var result = userManager.AddToRoleAsync(adminUser, Role.Admin.ToString()).Result;

            if (!result.Succeeded)
            {
                logger.LogError("Could not add Admin role to default admin user. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => $"{e.Code} => {e.Description}")));
                throw new Exception("Could not add Admin role to default admin user.");
            }
        }

        var categories = new List<Category>
        {
            new Category
            {
                Name = "Electrónica",
                Description = "Todos los productos electrónicos, desde smartphones y laptops hasta televisores y dispositivos inteligentes para el hogar. ¡Mantén tu vida conectada y actualizada!",
                IsNew = true
            },
            new Category
            {
                Name = "Moda",
                Description = "Desde ropa y accesorios hasta zapatos y joyas, tenemos todo lo que necesitas para lucir bien y sentirte cómodo en cualquier ocasión. ¡Descubre tu propio estilo y muestra al mundo tu mejor versión!",
                IsNew = true
            },
            new Category
            {
                Name = "Hogar y Jardín",
                Description = "Todos los productos para el hogar y el jardín, desde muebles y decoración hasta herramientas y suministros de jardinería. ¡Crea un espacio acogedor y agradable para ti y tu familia, y convierte tu hogar en un oasis de tranquilidad y felicidad!"
            }
        };

        for (int i = 0; i < categories.Count; i++)
        {
            var foundCategory = context.Categories.FirstOrDefault(c => c.Name == categories[i].Name);

            if (foundCategory != null)
            {
                categories[i] = foundCategory;
                continue;
            }

            context.Categories.Add(categories[i]);
            context.SaveChanges();
        }

        var brands = new List<Brand>
        {
            new Brand
            {
                Name = "Apple",
                Logo = new Image
                {
                    Name = "apple",
                    Path = "apple.png",
                    Width = 2424,
                    Height = 829,
                    SizeInBytes = 39873
                },
                ShouldPromote = true
            },
            new Brand
            {
                Name = "Samsung",
                Logo = new Image
                {
                    Name = "samsung",
                    Path = "samsung.png",
                    Width = 2500,
                    Height = 830,
                    SizeInBytes = 73632
                },
                ShouldPromote = true
            },
            new Brand
            {
                Name = "Microsoft",
                Logo = new Image
                {
                    Name = "microsoft",
                    Path = "microsoft.png",
                    Width = 2500,
                    Height = 534,
                    SizeInBytes = 46974
                },
                ShouldPromote = true
            },
            new Brand
            {
                Name = "Google",
                Logo = new Image
                {
                    Name = "google",
                    Path = "google.png",
                    Width = 2500,
                    Height = 816,
                    SizeInBytes = 79155
                },
                ShouldPromote = true
            },
            new Brand
            {
                Name = "Lenovo",
                Logo = new Image
                {
                    Name = "lenovo",
                    Path = "lenovo.png",
                    Width = 2500,
                    Height = 833,
                    SizeInBytes = 26668
                },
                ShouldPromote = true
            }
        };

        for (int i = 0; i < brands.Count; i++)
        {
            var foundBrand = context.Brands.FirstOrDefault(c => c.Name == brands[i].Name);

            if (foundBrand != null)
            {
                brands[i] = foundBrand;
                continue;
            }

            context.Brands.Add(brands[i]);
            context.SaveChanges();
        }

        var products = new List<Product>
        {
            new Product
            {
                Name = "iPhone 12",
                Description = "The latest iPhone from Apple",
                Price = 799.99m,
                SalePrice = 749.99m,
                DocumentationLink = "www.apple.com/iphone12",
                Tags = "Apple, iPhone, Smartphone",
                ModelNumber = "A2341",
                SKU = "IPH12-64GB-BLK",
                VATPercentage = 0.16m,
                Rating = 5,
                OnSale = true,
                Brand = brands[0],
                Category = categories[0],
                SliderImage = new Image
                {
                    Name = "iphone",
                    Path = "iphone.jpg",
                    Width = 1720,
                    Height = 820,
                    SizeInBytes = 70457
                },
                SliderOrder = 1,
                SliderPrimaryColor = "#0163d2",
                SliderSecondaryColor = "#ffffff",
                ShowInSlider = true,
                Dimension = new ProductDimension
                {
                    DepthInCentimeters = 10,
                    HeightInCentimeters = 20,
                    WeightInKilograms = 30,
                    WidthInCentimeters = 40
                }
            },
            new Product
            {
                Name = "Samsung Galaxy S21",
                Description = "The latest Samsung Galaxy",
                Price = 799.99m,
                SalePrice = 749.99m,
                DocumentationLink = "www.samsung.com/galaxys21",
                Tags = "Samsung, Galaxy, Smartphone",
                ModelNumber = "S21-64GB-BLK",
                SKU = "SAMS21-64GB-BLK",
                VATPercentage = 0.16m,
                Rating = 4,
                OnSale = true,
                Brand = brands[1],
                Category = categories[0],
                Dimension = new ProductDimension
                {
                    DepthInCentimeters = 20,
                    HeightInCentimeters = 10,
                    WeightInKilograms = 90,
                    WidthInCentimeters = 30
                }
            },
            new Product
            {
                Name = "MacBook Pro",
                Description = "The latest MacBook Pro from Apple",
                Price = 1499.99m,
                SalePrice = 1399.99m,
                DocumentationLink = "www.apple.com/macbookpro",
                Tags = "Apple, MacBook, Laptop",
                ModelNumber = "M1234",
                SKU = "MBP-16GB-BLK",
                VATPercentage = 0.16m,
                Rating = 4,
                OnSale = true,
                Brand = brands[0],
                Category = categories[0],
                SliderImage = new Image
                {
                    Name = "macbook",
                    Path = "macbook.jpg",
                    Width = 1720,
                    Height = 820,
                    SizeInBytes = 115240
                },
                SliderPrimaryColor = "#0163d2",
                SliderSecondaryColor = "#ffffff",
                SliderOrder = 2,
                ShowInSlider = true,
                Dimension = new ProductDimension
                {
                    DepthInCentimeters = 4,
                    HeightInCentimeters = 10,
                    WeightInKilograms = 20,
                    WidthInCentimeters = 73
                }
            },
            new Product
            {
                Name = "Surface Laptop",
                Description = "The latest Surface Laptop from Microsoft",
                Price = 999.99m,
                SalePrice = 949.99m,
                DocumentationLink = "www.microsoft.com/surfacelaptop",
                Tags = "Microsoft, Surface, Laptop",
                ModelNumber = "S1234",
                SKU = "SFL-8GB-BLK",
                VATPercentage = 0.16m,
                Rating = 4,
                OnSale = true,
                Brand = brands[2],
                Category = categories[0],
                Dimension = new ProductDimension
                {
                    DepthInCentimeters = 2,
                    HeightInCentimeters = 9,
                    WeightInKilograms = 15,
                    WidthInCentimeters = 70
                }
            },
            new Product
            {
                Name = "iPhone X",
                Description = "The latest iPhone from Apple",
                Price = 999.99m,
                SalePrice = 949.99m,
                DocumentationLink = "www.apple.com/iphonex",
                Tags = "Apple, iPhone, Mobile",
                ModelNumber = "I1234",
                SKU = "IPHX-64GB-BLK",
                VATPercentage = 0.16m,
                Rating = 4,
                OnSale = true,
                Brand = brands[0],
                Category = categories[0],
                Dimension = new ProductDimension
                {
                    DepthInCentimeters = 1,
                    HeightInCentimeters = 7,
                    WeightInKilograms = 0.5m,
                    WidthInCentimeters = 14
                }
            },
            new Product
            {
                Name = "Pixel 5",
                Description = "The latest Pixel from Google",
                Price = 799.99m,
                SalePrice = 749.99m,
                DocumentationLink = "www.google.com/pixel5",
                Tags = "Google, Pixel, Mobile",
                ModelNumber = "G1234",
                SKU = "PXL-128GB-BLK",
                VATPercentage = 0.16m,
                Rating = 4,
                OnSale = true,
                Brand = brands[3],
                Category = categories[0],
                Dimension = new ProductDimension
                {
                    DepthInCentimeters = 1,
                    HeightInCentimeters = 8,
                    WeightInKilograms = 0.6m,
                    WidthInCentimeters = 15
                }
            },
            new Product
            {
                Name = "ThinkPad X1 Carbon",
                Description = "The latest ThinkPad X1 Carbon from Lenovo",
                Price = 1299.99m,
                SalePrice = 1199.99m,
                DocumentationLink = "www.lenovo.com/thinkpadx1carbon",
                Tags = "Lenovo, ThinkPad, Laptop",
                ModelNumber = "L1234",
                SKU = "TPX1-16GB-BLK",
                VATPercentage = 0.16m,
                Rating = 3,
                OnSale = true,
                Brand = brands[4],
                Category = categories[0],
                Dimension = new ProductDimension
                {
                    DepthInCentimeters = 3,
                    HeightInCentimeters = 8,
                    WeightInKilograms = 1.5m,
                    WidthInCentimeters = 72
                }
            }
        };

        products[0].Images.Add(new ProductImage
        {
            Image = new Image
            {
                Name = "iphone12",
                Path = "iphone12.jpg",
                Width = 300,
                Height = 400,
                SizeInBytes = 18195
            }
        });

        products[0].Images.Add(new ProductImage
        {
            Image = new Image
            {
                Name = "iphone12_2",
                Path = "iphone12_2.jpg",
                Width = 300,
                Height = 400,
                SizeInBytes = 12791
            }
        });

        products[1].Images.Add(new ProductImage
        {
            Image = new Image
            {
                Name = "samsung",
                Path = "samsung.jpeg",
                Width = 309,
                Height = 400,
                SizeInBytes = 24576
            }
        });

        products[2].Images.Add(new ProductImage
        {
            Image = new Image
            {
                Name = "macbook",
                Path = "macbook.jpeg",
                Width = 904,
                Height = 840,
                SizeInBytes = 113740
            }
        });

        products[0].BuyedWithProducts.Add(new BuyedWithProduct
        {
            BuyedWith = products[2]
        });

        for (int i = 0; i < products.Count; i++)
        {
            var foundProduct = context.Products.FirstOrDefault(c => c.Name == products[i].Name);

            if (foundProduct != null)
            {
                products[i] = foundProduct;
                continue;
            }

            context.Products.Add(products[i]);
            context.SaveChanges();
        }

        var productShowcaseCollections = new List<ProductShowcaseCollection>
        {
            new ProductShowcaseCollection
            {
                Caption = "Lo último de lo último",
                Title = "Apple",
                Type = ProductShowcaseType.Simple,
                Order = 1
            },
            new ProductShowcaseCollection
            {
                Caption = "Conecta con tus seres queridos",
                Title = "Celulares",
                Type = ProductShowcaseType.Compact,
                Order = 2
            },
            new ProductShowcaseCollection
            {
                Caption = "Compra al mejor precio",
                Title = "Ofertas especiales",
                Type = ProductShowcaseType.Tab,
                Order = 3
            },
            new ProductShowcaseCollection
            {
                Caption = "Los productos con más likes",
                Title = "Instagram",
                Type = ProductShowcaseType.Cover,
                Order = 4
            }
        };

        productShowcaseCollections[0].Items.Add(new ProductShowcaseCollectionItem
        {
            Product = products[0]
        });

        productShowcaseCollections[0].Items.Add(new ProductShowcaseCollectionItem
        {
            Product = products[2]
        });

        productShowcaseCollections[1].Items.AddRange(products.Select(p => new ProductShowcaseCollectionItem
        {
            Product = p
        }));

        productShowcaseCollections[2].Items.AddRange(products.Select(p => new ProductShowcaseCollectionItem
        {
            Product = p
        }));

        productShowcaseCollections[3].Items.AddRange(products.Select(p => new ProductShowcaseCollectionItem
        {
            Product = p
        }));

        for (int i = 0; i < productShowcaseCollections.Count; i++)
        {
            var foundProductShowcaseCollection = context.ProductShowcaseCollections.FirstOrDefault(c => c.Title == productShowcaseCollections[i].Title);

            if (foundProductShowcaseCollection != null)
            {
                productShowcaseCollections[i] = foundProductShowcaseCollection;
                continue;
            }

            context.ProductShowcaseCollections.Add(productShowcaseCollections[i]);
            context.SaveChanges();
        }

        var serviceBanners = new List<ServiceBanner>
        {
            new ServiceBanner
            {
                Icon = "customer",
                Name = "Servicio al cliente",
                Description = "El mejor servicio al cliente.",
                Order = 1
            },
            new ServiceBanner
            {
                Icon = "shop",
                Name = "Recoge en cualquier sucursal",
                Description = "Envío gratuito a toda la república mexicana.",
                Order = 2
            },
            new ServiceBanner
            {
                Icon = "secure-payment",
                Name = "Pagos Seguros",
                Description = "Aceptamos las tarjetas más comúnes.",
                Order = 3
            },
            new ServiceBanner
            {
                Icon = "return",
                Name = "Garantía de devolución",
                Description = "Garantía de 30 días para devolver el producto.",
                Order = 4
            }
        };

        for (int i = 0; i < serviceBanners.Count; i++)
        {
            var foundServiceBanner = context.ServiceBanners.FirstOrDefault(c => c.Name == serviceBanners[i].Name);

            if (foundServiceBanner != null)
            {
                serviceBanners[i] = foundServiceBanner;
                continue;
            }

            context.ServiceBanners.Add(serviceBanners[i]);
            context.SaveChanges();
        }

        var settings = new List<Setting>
        {
            new Setting
            {
                Type = SettingType.SHOP_STORE,
                Value = "Tecno Fácil"
            },
            new Setting
            {
                Type = SettingType.CONTACT_PHONE,
                Value = "+ 185659635"
            },
            new Setting
            {
                Type = SettingType.CONTACT_ADDRESS,
                Value = "1418 Riverwood Drive, Suite 3245 Cottonwood, CA 96052, United States"
            },
            new Setting
            {
                Type = SettingType.CONTACT_EMAIL,
                Value = "voxo123@gmail.com"
            },
            new Setting
            {
                Type = SettingType.TOP_BAR_LEYEND_1,
                Value = "Envío gratis a toda la república mexicana"
            },
            new Setting
            {
                Type = SettingType.TOP_BAR_LEYEND_2,
                Value = "20% de descuento en electrónicos"
            },
            new Setting
            {
                Type = SettingType.TOP_BAR_LEYEND_3,
                Value = "Todos los precios están en MXN"
            },
            new Setting
            {
                Type = SettingType.THEME_PRIMARY_COLOR,
                Value = "#0163d2"
            },
            new Setting
            {
                Type = SettingType.STRIPE_PUBLISHABLE_KEY,
                Value = "pk_test_51KjsQqHcSFdwQVcwmHlBn3eRzKrS93uGyI80Pbrqtx5v54MULQXOfRqvYg8IQmEt3faLqgFitOqnYLwaGvZt3ZAW00t9AoqMKM"
            },
            new Setting
            {
                Type = SettingType.STRIPE_SECRET_KEY,
                Value = "sk_test_51KjsQqHcSFdwQVcwaDOgHTBzSsiqr8iszX9146F0XXdMMVeQJzBsSUCIN7qpsUCNwiLjVgW0Lte4VXVPYbByNGxZ00ctmTAM5Q"
            },
            new Setting
            {
                Type = SettingType.STRIPE_TRANSFER_FEE,
                Value = "false"
            },
            new Setting
            {
                Type = SettingType.STRIPE_FIXED_FEE,
                Value = "3"
            },
            new Setting
            {
                Type = SettingType.STRIPE_PERCENT_FEE,
                Value = "3.6"
            },
            new Setting
            {
                Type = SettingType.STRIPE_VAT_PERCENT_OVER_FEE,
                Value = "16"
            }
        };

        for (int i = 0; i < settings.Count; i++)
        {
            var foundSetting = context.Settings.FirstOrDefault(c => c.Type == settings[i].Type);

            if (foundSetting != null)
            {
                settings[i] = foundSetting;
                continue;
            }

            context.Settings.Add(settings[i]);
            context.SaveChanges();
        }
    }
    #endregion
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseHangfireDashboard();

app.MapControllerRoute(
    name: "ProductDetail",
    pattern: "p/{productName}",
    defaults: new { controller = "Products", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
