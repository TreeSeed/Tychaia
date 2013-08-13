// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using Protogame;

namespace Tychaia
{
    public class CameraCommand : ICommand
    {
        public string[] Names { get { return new[] { "camera" }; } }
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Configure the camera."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            var world = gameContext.World as TychaiaGameWorld;
            if (world == null)
                return "Not currently in-game.";
            var isometricCamera = world.IsometricCamera;

            if (parameters.Length < 1)
                return "Not enough parameters.";

            switch (parameters[0].ToLower())
            {
                case "help":
                    return @"show - Show the current settings of the camera.
enable-rotation - Enable rotation in the camera.
disable-rotation - Disable rotation in the camera.
enable-orthographic - Enable orthographic view in the camera.
disable-orthographic - Disable orthographic view in the camera.
set-distance <dist> - Set the distance of the camera to the focal point.
set-angle <dist> - Set the vertical angle of the camera to the focal point.";
                case "enable-rotation":
                    isometricCamera.Rotation = true;
                    return "Rotation is enabled.";
                case "disable-rotation":
                    isometricCamera.Rotation = false;
                    return "Rotation is disabled and reset.";
                case "enable-orthographic":
                    isometricCamera.Orthographic = true;
                    return "Orthographic view is enabled.";
                case "disable-orthographic":
                    isometricCamera.Orthographic = false;
                    return "Orthographic view is disabled.";
                case "set-distance":
                    if (parameters.Length < 2)
                        return "Require 1 parameter.";
                    isometricCamera.Distance = Convert.ToInt32(parameters[1]);
                    return "Distance is now " + parameters[1] + ".";
                case "set-angle":
                    if (parameters.Length < 2)
                        return "Require 1 parameter.";
                    isometricCamera.VerticalAngle = Convert.ToInt32(parameters[1]);
                    return "Angle is now " + parameters[1] + ".";
                case "show":
                    return "Camera distance is " + isometricCamera.Distance + @".
Camera vertical angle is " + isometricCamera.VerticalAngle + @".
Camera focal point is " + isometricCamera.CurrentFocus + @".

Use 'camera help' for a list of commands.";
                default:
                    return "Unknown command (try `help`).";
            }
        }
    }
}

