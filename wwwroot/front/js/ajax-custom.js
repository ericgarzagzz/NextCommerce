var substringMatcher = function (strs) {
    return function findMatches(q, cb) {
        var matches, substringRegex;
        matches = [];
        substrRegex = new RegExp(q, "i");
        $.each(strs, function (i, str) {
            if (substrRegex.test(str)) {
                matches.push(str);
            }
        });
        cb(matches);
    };
};

var states = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.obj.whitespace("name"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
        url: "/Home/GetAllProducts"
    }
});

states.initialize();

$(".typeahead").typeahead({
    hint: true,
    highlight: true,
    minLength: 1
}, {
    name: "states",
    display: "name",
    source: states.ttAdapter(),
    templates: {
        empty: [
            '<div class="empty-message">',
            "No se encontraron resultados",
            "</div>",
        ].join("\n"),
        suggestion: function (data) {
            var image = data.image != null ? `${$("#product-image-url").val()}${data.image}` : $("#no-image-search-url").val();
            var priceDiv = data.showOldPrice ? `<div><span>${data.salePrice}</span><span class="font-light ms-1 text-decoration-line-through">${data.price}</span></div>` : `<span>${data.salePrice}</span>`;

            return (
                `<a href="${$("#product-detail-url").val().replace("ProductName", data.name)}" class="man-section"><div class="image-section"><img src=` +
                image +
                '></div><div class="description-section"><h4>' +
                data.name +
                "</h4>" +
                priceDiv +
                "</div></a>"
            );
        },
    },
});