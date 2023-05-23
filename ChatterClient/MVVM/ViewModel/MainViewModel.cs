using ChatterClient.MVVM.Core;
using ChatterClient.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatterClient.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        private string _message;
        public string Message 
        { 
            get { return _message; }
            set 
            {
                _message = value;
                OnPropertyChanged();
            } 
        }
        public string Username { get; set; }
        

        private Server _server;

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            _server = new Server();

            _server.ConnectedEvent += UserConnected;
            _server.RecieveMessageEvent += MessageRecieved;
            _server.UserDisconnectedEvent += UserDisconnected;

            ConnectToServerCommand = new RelayCommand(
                o => _server.ConnectToServer(Username),
                o => !string.IsNullOrEmpty(Username)
            );

            SendMessageCommand = new RelayCommand(
                o => SendMessage(),
                o => !string.IsNullOrEmpty(Message)
            );
        }

        private void SendMessage()
        {
            _server.SendMessageToServer(Message);
            Message = "";
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadContent(),
                UID = _server.PacketReader.ReadContent()
            };

            if (!Users.Any(o => o.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        private void UserDisconnected()
        {
            string uid = _server.PacketReader.ReadContent();
            UserModel user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageRecieved()
        {
            string msg = _server.PacketReader.ReadContent();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

    }
}
