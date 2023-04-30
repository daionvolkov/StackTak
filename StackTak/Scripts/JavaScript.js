


// Hide/Show input form Getway in Create form
function toggleGatwayInputForm() {
    var radio1 = document.getElementById("radioAggregationBtn");
    var radio2 = document.getElementById("radioAccessBtn");
    var div = document.getElementById("gateway_form");
    if (radio1.checked) {
        div.style.display = "none";
    } else if (radio2.checked) {
        div.style.display = "block";
    }
}