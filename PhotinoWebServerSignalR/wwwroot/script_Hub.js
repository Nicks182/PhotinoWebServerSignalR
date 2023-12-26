
var G_ConnectionUrl = "/MessageHub";

var G_HubConnection = new signalR.HubConnectionBuilder().withUrl(G_ConnectionUrl).build();

// Setup handler for message from server...    
G_HubConnection.on("ClientMessageHandler", _Message_Receive);

function _Message_Receive(P_MessageObject)
{
    console.log(P_MessageObject);

    switch (P_MessageObject.MessageType)
    {
        case "Greetings":
            _Handle_Greetings(P_MessageObject);
            break;

        case "_DoNothing":
            _Handle_DoNothing(P_MessageObject);
            break;
    }
}


function _Message_Send(P_MessageObject)
{
    document.getElementById("Div_Greetings").innerHTML = "";

    G_HubConnection.invoke("ServerMessageHandler", _ToJSONString(P_MessageObject));

}

function _ToJSONString(P_MessageObject)
{
    return JSON.stringify(P_MessageObject);
}



document.addEventListener("DOMContentLoaded", () =>
{
    G_HubConnection.start().then(function () 
    {
        console.log("Hub Status: " + G_HubConnection.state);
    });
});

