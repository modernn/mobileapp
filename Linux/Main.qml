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
    Row {
        anchors.top: parent.top
        anchors.right: parent.right
        Button {
            text: "Sync"
            onClicked: toggl.sync()
        }
        Button {
            text: "Log out"
            onClicked: toggl.logout()
        }
    }

    Text {
        id: status
        text: "Status: " + toggl.status
    }
    Text {
        id: error
        anchors.top: status.bottom
        text: "Error: " + toggl.error
    }

    Row {
        id: tabs
        anchors.top: error.bottom
        spacing: 3
        Repeater {
            model: toggl.tabs ? Net.toListModel(toggl.tabs) : null
            Text {
                text: modelData
            }
        }
    }
    Row {
        id: timer
        anchors.top: tabs.bottom
        Text {
            text: JSON.stringify(toggl.runningTimeEntry)
        }
        Text {
            text: " : "
        }
        Text {
            text: toggl.runningTimeEntry.description
        }
    }
    ListView {
        id: tes

        clip: true
        anchors.top: timer.bottom
        width: parent.width
        anchors.bottom: parent.bottom

        spacing: 6

        model: Net.toListModel(toggl.timeEntries2)
        
        delegate: Rectangle {
            border {
                width: 1
                color: "light gray"
            }
            width: tes.width
            height: delegateLayout.height
            RowLayout {
                id: delegateLayout
                width: parent.width
                ColumnLayout {
                    Text {
                        text: modelData.description.length <= 0 ? "No description" : modelData.description
                        color: modelData.description.length <= 0 ? "gray" : "black"
                    }
                    Text {
                        text: modelData.project ? modelData.project.name : ""
                        color: modelData.project ? modelData.project.color : "transparent"
                    }
                }
                Item {
                    Layout.fillWidth: true
                }
                Text {
                    Layout.alignment: Qt.AlignRight | Qt.AlignHCenter
                    text: modelData.duration
                }
            }
            //text: modelData? JSON.stringify(modelData) : "null"
        }
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
        visible: toggl.status !== "LOGIN VIEW" && toggl.status !== "MAIN TAB BAR VIEW"
        anchors.centerIn: parent
    }
}