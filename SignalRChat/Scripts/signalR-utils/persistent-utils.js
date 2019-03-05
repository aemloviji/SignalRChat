var userHasActiveSession = false;

$(function () {
    var myConnection = $.connection("/chat");

    myConnection.received(function (data) {


        $("#chatroom ul").append("<li><strong>" + htmlEncode(data.Name) +
            '</strong>: ' + htmlEncode(data.Message) + "</li>");
    });

    registerJsEvents(myConnection);
});

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
                chatConnection.start().done(function () {
                    userHasActiveSession = true;

                    bindClickEventToSendMessageButton();
                    activateChatRoom();

                    var clientPayload = { type: "UserConnectedRequest", userName: $("#inputUserName").val().trim() };
                    chatConnection.send(JSON.stringify(clientPayload));
                });
            }
            else {
                alert("Please enter a username!");
            }
        });
    }

    function bindClickEventToSendMessageButton() {
        $('#buttonSendMessage').click(function () {
            chatConnection.send(JSON.stringify({ name: $('#hiddenUserName').val(), message: $('#inputMessage').val() }));
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