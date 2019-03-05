var userHasActiveSession = false;

$(function () {
    var myConnection = $.connection("/chat");

    myConnection.received(function (data) {
        handlePayload(data);
    });

    registerJsEvents(myConnection);
});

function handlePayload(data) {
    switch (data.Type) {
        case "UserConnectedPayload":
            //$("#chat-users")
            //    .append('<li id="' + id + '" class="list-group-item">' + data.UserName + '</li>');
            break;
        case "NewMessagePayload":
            createNewMessageElement(data.UserName, data.Message);
            scroolMessagesPane();
            break;
        default:
            break;
    }
}

function deactivateUserActiveSession(id) {
    var currentUserId = $('#hiddenUserId').val();
    if (currentUserId === id) {
        userHasActiveSession = false;
    }
}

function registerJsEvents(chatConnection) {
    bindClickEventToLoginButton();

    function bindClickEventToLoginButton() {
        $("#buttonLogin").click(function () {
            var name = $("#inputUserName").val().trim();
            if (name.length > 0) {
                chatConnection.start().done(function (data) {
                    console.info(data.id);
                    userHasActiveSession = true;

                    bindClickEventToSendMessageButton();
                    activateChatRoom();

                    var clientRequest = { type: "UserConnectedRequest", userName: $("#inputUserName").val().trim() };
                    chatConnection.send(JSON.stringify(clientRequest));
                });
            }
            else {
                alert("Please enter a username!");
            }
        });
    }

    function bindClickEventToSendMessageButton() {
        $('#buttonSendMessage').click(function () {
            var clientRequest = {
                type: "NewMessageRequest",
                userName: $("#inputUserName").val().trim(),
                message: $('#inputMessage').val().trim()
            };
            chatConnection.send(JSON.stringify(clientRequest));
            $('#inputMessage').val('');
        });
    }
}

function removeUserById(id) {
    $('#' + id).remove();
}

function activateChatRoom() {
    $('#block-login').hide();
    $('#block-chat').show();
}

function showUserList(allActiveUsers) {
    for (i = 0; i < allActiveUsers.length; i++) {
        addUser(allActiveUsers[i].ConnectionId, allActiveUsers[i].Name);
    }
}

function createNewMessageElement(name, message) {
    var encodedName = htmlEncode(name);
    var encodedMessage = htmlEncode(message);
    $('#chat-messages').append('<p><b class="badge badge-pill badge-light">' + encodedName + '</b>: ' + encodedMessage + '</p>');
}

function scroolMessagesPane() {
    var chatMessagesVerticalScroolHeight = $('#chat-messages').prop('scrollHeight');
    $('#chat-messages').prop('scrollTop', chatMessagesVerticalScroolHeight);
}

function saveConnectedUserInfo(id, userName) {
    $('#hiddenUserId').val(id);
    $('#hiddenUserName').val(userName);
}

function greetUser(userName) {
    $('#greeting-section').html('<p>Welcome, ' + userName + '</p>');
}

function htmlEncode(value) {
    return String(value).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

function addUser(id, name) {
    var currentUserId = $('#hiddenUserId').val();

    if (currentUserId !== id) {

        $("#chat-users").append('<li id="' + id + '" class="list-group-item">' + name + '</li>');
    }
}