
function callDotNet()
{
    var Message =
    {
        Message: "Hi from Script!",
        MessageType: "Greetings"
    };

    _Message_Send(Message);
}

function _DoNothing()
{
    var Message =
    {
        Message: "",
        MessageType: "_DoNothing"
    };

    _Message_Send(Message);
}

function _Handle_Greetings(P_MessageObject)
{
    
    document.getElementById("Div_Greetings").innerHTML = P_MessageObject.Message;
}

function _Handle_DoNothing(P_MessageObject)
{
    alert(P_MessageObject.Message);
}