﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Utilities;
using Avalanche;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class Login : ContentView, IRenderable
    {
        public Login()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            return this;
        }

        private async void btnSubmit_Clicked( object sender, EventArgs e )
        {
            var response = await RockClient.LogIn( username.Text, password.Text );
        }

        private async void btnLogout_Clicked( object sender, EventArgs e )
        {
            RockClient.Logout();
        }
    }
}