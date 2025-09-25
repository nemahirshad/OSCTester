using UnityEngine;
using TMPro;
using OscJack;
using System; // Ensure System is included for the Action type

public class ResolumeOSCController : MonoBehaviour
{
    // Public UI references
    public TMP_InputField clientIpInputField;
    public TMP_InputField clientPortInputField;
    public TMP_InputField serverPortInputField;
    public TMP_Text feedbackText;

    // OSC objects
    private OscClient client;
    private static OscServer server;

    void OnDisable()
    {
        if (client != null)
        {
            client.Dispose();
            client = null;
        }
    }

    private void OnDestroy()
    {
        if (server != null)
        {
            server.Dispose();
            server = null;
        }
    }

    public void ConnectServer()
    {
        int serverPort;

        if (!int.TryParse(serverPortInputField.text, out serverPort))
        {
            feedbackText.text = "Error: Invalid server port number.";
            return;
        }

        if (server != null)
        {
            server.Dispose();
            server = null;
        }

        server = new OscServer(serverPort);
        server.MessageDispatcher.AddCallback(
            "/resolume/feedback",
            (string address, OscDataHandle data) => {
                // Use the new dispatcher to update the UI safely
                MainThreadDispatcher.RunOnMainThread(() => {
                    feedbackText.text = $"Received message from Resolume on address: {address}";
                });
            }
        );

        feedbackText.text = $"OSC Server is now listening on port: {serverPort}.";
    }

    public void SendTestMessage()
    {
        string ipAddress = clientIpInputField.text;
        int portNumber;

        if (!int.TryParse(clientPortInputField.text, out portNumber))
        {
            feedbackText.text = "Error: Invalid client port number.";
            return;
        }

        if (client != null)
        {
            client.Dispose();
        }

        client = new OscClient(ipAddress, portNumber);

        client.Send("/composition/layers/1/clips/1/connect", 1);

        feedbackText.text = $"Sent message to {ipAddress}:{portNumber}.";
    }

    // Inside your ResolumeOSCController class

    public void TriggerClip()
    {
        string ipAddress = clientIpInputField.text;
        int portNumber;

        if (!int.TryParse(clientPortInputField.text, out portNumber))
        {
            feedbackText.text = "Error: Invalid client port number.";
            return;
        }

        if (client != null)
        {
            client.Dispose();
        }

        client = new OscClient(ipAddress, portNumber);

        client.Send("/composition/layers/1/clips/1/connect", 1);

        feedbackText.text = $"Sent trigger message to Resolume on: {ipAddress}:{portNumber}.";
    }
}