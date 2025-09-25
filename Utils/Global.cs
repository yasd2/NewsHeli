global using System;
global using System.IO;
global using System.Linq;
global using System.Xml.Linq;
global using System.Reflection;
global using System.Drawing;
global using System.Windows.Forms;
global using System.Collections.Generic;
global using Rage;
global using Rage.Native;
global using NewsHeli.Utils;
global using NewsHeli.Utils.Extentions;
global using LSPD_First_Response.Mod.API;
global using Object = Rage.Object;


/// <summary>
/// Global usings is very helpful, it requires a newer C# language version, but makes live easier.
/// Thx to Astros open source code for showing this. https://github.com/AstroBurgers
/// </summary>
internal class Global { internal static Ped Player => Game.LocalPlayer.Character; }
