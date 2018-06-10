using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class SocketT2h
{
    public Socket _Socket { get; set; }
    public string _Name { get; set; }
    public SocketT2h(Socket socket)
    {
        this._Socket = socket;
    }
}


public class SocketV2 : MonoBehaviour {
    private byte[] _buffer = new byte[1024];
    public List<SocketT2h> __ClientSockets { get; set; }
    List<string> _names = new List<string>();
    private Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private Socket connectedSocket;

    // Use this for initialization
    void Start () {
        __ClientSockets = new List<SocketT2h>();
        SetupServer();
    }

    private void SetupServer()
    {
        _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1234));
        _serverSocket.Listen(1);
        _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
    }

    private void AppceptCallback(IAsyncResult ar)
    {
        Socket socket = _serverSocket.EndAccept(ar);
        connectedSocket = socket;
        __ClientSockets.Add(new SocketT2h(socket));
        Debug.Log(socket.RemoteEndPoint.ToString());

        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
    }


    private void ReceiveCallback(IAsyncResult ar)
    {

        Socket socket = (Socket)ar.AsyncState;
        if (socket.Connected)
        {
            int received;
            try
            {
                received = socket.EndReceive(ar);
            }
            catch (Exception)
            {
                // client đóng kết nối
                for (int i = 0; i < __ClientSockets.Count; i++)
                {
                    if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                    {
                        __ClientSockets.RemoveAt(i);
                        
                    }
                }
                // xóa trong list
                return;
            }
            if (received != 0)
            {
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                string text = Encoding.ASCII.GetString(dataBuf);
             
                string reponse = string.Empty;
                //if (text.Contains("@@"))
                //{
                //    for (int i = 0; i < list_Client.Items.Count; i++)
                //    {
                //        if (socket.RemoteEndPoint.ToString().Equals(__ClientSockets[i]._Socket.RemoteEndPoint.ToString()))
                //        {
                //            list_Client.Items.RemoveAt(i);
                //            list_Client.Items.Insert(i, text.Substring(1, text.Length - 1));
                //            __ClientSockets[i]._Name = text;
                //            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                //            return;
                //        }
                //    }
                //}

                for (int i = 0; i < __ClientSockets.Count; i++)
                {
                    if (socket.RemoteEndPoint.ToString().Equals(__ClientSockets[i]._Socket.RemoteEndPoint.ToString()))
                    {
                        Debug.Log("\n" + __ClientSockets[i]._Name + ": " + text);
                    }
                }
                if (text == "bye")
                {
                    return;
                }
                reponse = "server da nhan" + text;
                Sendata(socket, reponse);
            }
            else
            {
                for (int i = 0; i < __ClientSockets.Count; i++)
                {
                    if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                    {
                        __ClientSockets.RemoveAt(i);
                        //lb_soluong.Text = "Số client đang kết nối: " + __ClientSockets.Count.ToString();
                    }
                }
            }
        }
        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
    }

    public void Sendata(Socket socket, string noidung)
    {
        byte[] data = Encoding.ASCII.GetBytes(noidung);
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
    }

    public void SendData(string noidung)
    {
        byte[] data = Encoding.ASCII.GetBytes(noidung);
        connectedSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), connectedSocket);
        _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
    }

    private void SendCallback(IAsyncResult AR)
    {
        Socket socket = (Socket)AR.AsyncState;
        socket.EndSend(AR);
    }
    
    // Update is called once per frame
    void Update () {
		
	}
}
