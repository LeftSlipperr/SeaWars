using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая_Морской_бой
{
    public partial class Form1 : GameLogic
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Морской бой"; // название формы
            Init();
            
        }

    }
}
