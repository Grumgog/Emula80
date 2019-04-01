using System;


namespace Processor
{
    public static class StandartSettings
    {
        public static string PluggableDevicesFolderName { get => "Pluggable_Devices"; }
        public static string PluggableDevicesFolderPath { get => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                "\\" + PluggableDevicesFolderName + "\\"; }
        public static string PluggableInputDevicesName { get => "Input"; }
        public static string PluggableInputDevicesPath { get => PluggableDevicesFolderPath + PluggableInputDevicesName + "\\"; }
        public static string PluggableOutputDevicesName { get => "Out"; }
        public static string PluggableOutputDevicesPath { get => PluggableDevicesFolderPath + PluggableOutputDevicesName + "\\"; }
    }
}
