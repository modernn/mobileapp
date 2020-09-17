import QtQuick 2.12
import QtQuick.Layouts 1.12

Item {
    property QtObject timeEntry: null
    property bool editable: false
    Layout.preferredHeight: height
    height: editable ? timeEntryField.height * 2 + 6 : timeEntryText.height * 2 + 6

    Text {
        id: timeEntryText
        anchors {
            bottom: parent.verticalCenter
            bottomMargin: editable ? 10 : 1.5
            left: parent.left
            leftMargin: 6
            right: parent.right
        }
        Behavior on y { NumberAnimation { duration: 120 } }

        visible: timeEntry
        opacity: editable ? 0.0 : 1.0
        Behavior on opacity { NumberAnimation { duration: 120 } }
        text: timeEntry && timeEntry.description && timeEntry.description.length > 0 ? timeEntry.description : "+ Add description"
        color: timeEntry && timeEntry.description && timeEntry.description.length > 0 ? mainPalette.text : disabledPalette.text

        wrapMode: Text.WrapAtWordBoundaryOrAnywhere
        font.pixelSize: 12
        verticalAlignment: Text.AlignVCenter
    }

    RowLayout {
        id: projectLayout
        anchors {
            top: parent.verticalCenter
            topMargin: editable ? 9 : 1.5
            left: parent.left
            leftMargin: 6
            right: parent.right
        }
        Behavior on y { NumberAnimation { duration: 120 } }

        opacity: editable ? 0.0 : 1.0
        Rectangle {
            visible: timeEntry && timeEntry.project && timeEntry.project.name && timeEntry.project.name.length > 0
            height: 8
            width: height
            radius: height / 2
            color: timeEntry && timeEntry.project ? timeEntry.project.color : mainPalette.text
            Layout.alignment: Qt.AlignVCenter
        }
        Text {
            visible: timeEntry
            text: timeEntry && timeEntry.project && timeEntry.project.name.length > 0 ? timeEntry.project.name : "+ Add project"
            color: timeEntry && timeEntry.project && timeEntry.project.name.length > 0 ? (timeEntry.project.color.length ? timeEntry.project.color : mainPalette.text) : disabledPalette.text
            font.pixelSize: 12
        }
        Text {
            visible: timeEntry && timeEntry.task && timeEntry.task.name && timeEntry.task.name.length > 0
            text: timeEntry && timeEntry.task && timeEntry.task.name.length ? "- " + timeEntry.task.name : ""
            color: mainPalette.text
            font.pixelSize: 12
        }
        Text {
            visible: timeEntry && timeEntry.client && timeEntry.client.name && timeEntry.client.name.length > 0
            text: timeEntry && timeEntry.client && timeEntry.client.name ? "• " + timeEntry.client.name : ""
            color: mainPalette.text
            font.pixelSize: 12
        }
        Item {
            Layout.fillWidth: true
        }
    }

    TogglTextField {
        id: timeEntryField
        anchors {
            verticalCenter: timeEntryText.verticalCenter
            left: parent.left
            right: parent.right
        }

        visible: opacity > 0.0
        opacity: editable ? 1.0 : 0.0
        Behavior on opacity { NumberAnimation { duration: 120 } }
        text: timeEntry && timeEntry.description && timeEntry.description.length > 0 ? timeEntry.description : ""
        placeholderText: qsTr("Add description")
        onAccepted: {
            toggl.setTimeEntryDescription(timeEntry.GUID, timeEntryField.text)
        }
    }


    TogglTextField {
        id: projectField
        anchors {
            verticalCenter: projectLayout.verticalCenter
            left: parent.left
            right: parent.right
        }

        visible: opacity > 0.0
        opacity: editable ? 1.0 : 0.0
        Behavior on opacity { NumberAnimation { duration: 120 } }
        text: timeEntry && timeEntry.project && timeEntry.project.name.length > 0 ? timeEntry.project.name : ""
        placeholderText: qsTr("Select project")
    }

}
