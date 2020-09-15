//Main.qml
import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0

import toggl 1.0

ApplicationWindow {
    visible: true
    width: 640
    height: 480
    title: qsTr("Toggl Track")

    Text {
        id: status
        text: "Status: " + toggl.status
    }
    Text {
        id: error
        anchors.top: status.bottom
        text: "Error: " + toggl.error
    }
    
    Item {
        anchors.fill: parent
        visible: toggl.status === "LOGIN VIEW"
        Rectangle {
            anchors.fill: loginView
            anchors.margins: -6
            border.color: "gray"
            border.width: 1
        }
        ColumnLayout {
            id: loginView
            anchors.centerIn: parent
            TextField {
                id: username
                placeholderText: "Login"
            }
            TextField {
                id: password
                placeholderText: "Password"
            }
            Button {
                Layout.fillWidth: true
                text: "Log in"
                onClicked: toggl.login(username.text, password.text)
            }
        }
    }
    Text {
        text: "HERE BE DRAGONS"
        visible: toggl.status !== "LOGIN VIEW"
        anchors.centerIn: parent
    }
}