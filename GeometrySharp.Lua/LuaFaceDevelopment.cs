//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using GeometrySharp.Procedural;
//using LuaInterface;
//using System.Reflection;

//namespace GeometrySharp.Lua
//{
//    public class LuaFaceDevelopment
//        :FaceDevelopment
//    {
//        LuaInterface.Lua luaState;

//        public LuaFaceDevelopment(string script, string name)
//        {
//            luaState = new LuaInterface.Lua();
//            var m = this.GetType().GetMethod("Test", BindingFlags.NonPublic | BindingFlags.Instance);
//            luaState.RegisterFunction("Test", this, m);

//            LuaFunction func = luaState.LoadString(script, name);
//            func.Call();
//        }

//        private void Test(string s)
//        {
//            throw new NotImplementedException();
//        }

//        public override FaceDiminishment Apply(ProceduralFace face)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
