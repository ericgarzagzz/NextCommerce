const stripe = Stripe($("#stripe_pk").val(), {
    locale: "es-419"
});

initialize();

$("#payment-form").submit(handleSubmit);

// Fetches a payment intent and captures the client secret
async function initialize() {
    const clientSecret = $("#pi_client_secret").val();

    const appearance = {
        theme: 'stripe',
    };
    elements = stripe.elements({ appearance, clientSecret });

    const paymentElementOptions = {
        layout: "accordion"
    };

    const paymentElement = elements.create("payment", paymentElementOptions);
    paymentElement.mount("#payment-element");

    const shippingAddressElement = elements.create('address', {
        mode: 'shipping',
        allowedCountries: ["mx"]
    });
    shippingAddressElement.mount("#shipping-address-element");

    $("#confirmPaymentBtn").prop("disabled", false);
}

async function handleSubmit(e) {
    e.preventDefault();

    setLoading(true);

    const { error } = await stripe.confirmPayment({
        elements,
        confirmParams: {
            return_url: $("#process-success-payment-url").val(),
            receipt_email: $("#email").val()
        },
    });

    if (error.type === "card_error" || error.type === "validation_error") {
        showMessage(error.message);
    } else if (error.payment_intent && error.payment_intent.canceled_at && error.payment_intent.canceled_at != null) {
        showMessage("El cargo no se ha realizado ya que se ha abierto otra pantalla de pago, o se ha modificado el carrito. Por favor, presiona F5 o recarga esta página web para continuar.");
    } else {
        showMessage("An unexpected error occurred.");
    }

    setLoading(false);
}

function showMessage(messageText) {
    const messageContainer = document.querySelector("#payment-message");

    messageContainer.classList.remove("d-none");
    messageContainer.textContent = messageText;
}

function setLoading(isLoading) {
    $("#confirmPaymentBtn").prop("disabled", isLoading);
    $("#spinner").toggleClass("d-none", !isLoading);
}