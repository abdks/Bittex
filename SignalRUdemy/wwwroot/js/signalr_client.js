$(document).ready(function () {
    const receiveMessageForAllClientClientMethodCall = "ReceiveMessageForAllClient";
    const connection = new signalR.HubConnectionBuilder().withUrl("/exampleTypeSafeHub").configureLogging(signalR.LogLevel.Information).build();

    function start() {
        connection.start().then(() => {
            console.log("Hub ile Bağlantı Kuruldu");
            $("#connectionId").html(`Bağlantı Id: ${connection.connectionId}`);
        });
    }

    try {
        start();
    } catch (error) {
        console.error(error);
        setTimeout(() => start(), 5000);
    }

    connection.on(receiveMessageForAllClientClientMethodCall, (message) => {
        console.log("Gelen Mesaj", message);
        const [price, baseCurrency, cryptoCurrency, dailyPercent, cryptoImage] = message.split(" ");
        const targetRow = $(`#cryptoTableBody tr[data-crypto="${cryptoCurrency}"]`);
        if (targetRow.length > 0) {
            targetRow.find(".price").html(`${price} ${baseCurrency}`);
            targetRow.find(".daily-percent").html(`%${dailyPercent}`);
            targetRow.find(".crypto-image").attr("src", cryptoImage);

            $("#dailyPercentLabel").text(`Günlük Değişim: %${dailyPercent}`);
        } else {
            const row = `<tr data-crypto="${cryptoCurrency}">
                                    <td>${baseCurrency}</td>
                                    <td>${cryptoCurrency}</td>
                                    <td class="price">${price} ${baseCurrency}</td>
                                    <td class="daily-percent">%${dailyPercent}</td>
                                    <td><img class="crypto-image" src="${cryptoImage}" alt="${cryptoCurrency}"></td>
                                </tr>`;
            $("#cryptoTableBody").append(row);

            $("#dailyPercentLabel").text(`Günlük Değişim: %${dailyPercent}`);
        }
    });
});
