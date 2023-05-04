


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


//Ping switch
function pingAndUpdateStatus(ipAddress) {
    const switchUrl = `http://${ipAddress}/`;
    const statusDiv = document.querySelector('.is_available');

    fetch(switchUrl, { method: 'HEAD' })
        .then(response => {
            if (response.ok) {
                statusDiv.style.backgroundColor = 'green';
            } else {
                statusDiv.style.backgroundColor = 'red';
            }
        })
        .catch(() => {
            statusDiv.style.backgroundColor = 'red';
        });
}

const link = document.querySelector('a');

link.addEventListener('click', function (event) {
    event.preventDefault();
    const ipAddress = this.getAttribute('data-ip-address');
    pingAndUpdateStatus(ipAddress);
});

const myDiv = document.querySelector('.is_available');
myDiv.addEventListener('click', function () {
    myDiv.classList.toggle('white');
});