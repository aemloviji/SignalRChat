$(function () {
    var chat = $.connection.chatHub;
    chat.client.addMessage = function (name, message) {

        createNewMessageElement(name, message);
        scroolMessagesPane();
    };

    chat.client.onConnected = function (id, userName, allActiveUsers) {

        activateChatRoom();
        saveConnectedUserInfo(id, userName);
        greetUser(userName);
        showUserList(allActiveUsers);
    };

    chat.client.onNewUserConnected = function (id, name) {

        addUser(id, name);
    };

    chat.client.onUserDisconnected = function (id) {

        removeUserById(id);
    };

    $.connection.hub.start().done(function () {

        registerJsEvent(chat);
    });
});

function registerJsEvent(chat) {
    bindClickEventToLoginButton();
    bindClickEventToSendMessageButton();

    function bindClickEventToSendMessageButton() {
        $("#buttonLogin").click(function() {
            var name = $("#inputUserName").val().trim();
            if (name.length > 0) {
                chat.server.connect(name);
            }
            else {
                alert("Please enter a username!");
            }
        });
    }

    function bindClickEventToLoginButton() {
        $('#buttonSendMessage').click(function() {
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
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function addUser(id, name) {
    var currentUserId = $('#hiddenUserId').val();

    if (currentUserId !== id) {

        $("#chat-users").append('<li id="' + id + '" class="list-group-item">' + name + '</li>');
    }
}