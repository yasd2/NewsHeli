global using System;
global using System.Collections.Generic;
global using System.Linq;
global using Rage;
global using Rage.Native;
global using LSPD_First_Response.Engine;
global using LSPD_First_Response.Mod;
global using LSPD_First_Response.Tooling;
global using NewsHeli;
global using NewsHeli.Utils;
global using NewsHeli.Utils.Extentions;
global using System.Xml.Linq;
global using System.IO;
global using System.Drawing;
global using LSPD_First_Response.Mod.API;
global using System.Reflection;
global using Object = Rage.Object;

/// <summary>
/// Global usings is very helpful, it requires a newer C# language version, but makes live easier.
/// Thx to Astros open source code for showing this. https://github.com/AstroBurgers
/// </summary>
internal class Global { internal static Ped Player => Game.LocalPlayer.Character; }
