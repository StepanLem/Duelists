using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.__Internal_Assets.Architecture.MainMenu
{
    public class MainMenuEnterParams
    {
        public string Result { get; }

        public MainMenuEnterParams(string result) 
        { 
            Result = result;
        }
    }
}
