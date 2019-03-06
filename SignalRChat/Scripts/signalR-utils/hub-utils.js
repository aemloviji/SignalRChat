var userHasActiveSession = false;

$(function () {
    var chat = $.connection.chatHub;

    chat.client.addMessage = function (name, message) {

        createNewMessageElement(name, message);
        scroolMessagesPane();
    };

    chat.client.onConnected = function (id, userName, allActiveUsers) {
        userHasActiveSession = true;
        activateChatRoom();
        saveConnectedUserInfo(id, userName);
        greetUser(userName);
        generateUserList(allActiveUsers);
    };

    chat.client.onNewUserConnected = function (id, name) {
        if (userHasActiveSession) {
            addUser(id, name);
        }
    };

    chat.client.onUserDisconnected = function (id) {
        deactivateUserActiveSession(id);
        removeUserById(id);
    };

    $.connection.hub.start().done(function () {

        registerJsEvents(chat);
    });
});

function deactivateUserActiveSession(id) {
    var currentUserId = $('#hiddenUserId').val();
    if (currentUserId === id) {
        userHasActiveSession = false;
    }
}

function registerJsEvents(chat) {
    bindClickEventToLoginButton();
    bindClickEventToSendMessageButton();

    function bindClickEventToLoginButton() {
        $("#buttonLogin").click(function () {
            var name = $("#inputUserName").val().trim();
            if (name.length > 0) {
                chat.server.connect(name);
            }
            else {
                alert("Please enter a username!");
            }
        });
    }

    function bindClickEventToSendMessageButton() {
        $('#buttonSendMessage').click(function () {
            chat.server.send($('#hiddenUserName').val(), $('#inputMessage').val());
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

function generateUserList(allActiveUsers) {
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
        $("#chat-users")
            .append('<li id="' + id + '" class="list-group-item">' + name + '</li>');
    }
}