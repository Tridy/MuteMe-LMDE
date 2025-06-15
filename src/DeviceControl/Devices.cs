using System;
using System.Collections.Generic;
using System.Linq;

using HidSharp;

namespace DeviceControl;

public record DeviceInfo(int VendorId, int ProductId);

public static class Devices
{
    public static DeviceInfo? GetMuteMeDeviceInfo()
    {
        DeviceList? deviceList = DeviceList.Local;
        HidDevice[] allDevices = deviceList.GetHidDevices().ToArray();

        List<HidDevice> muteMeDevices = new();

        foreach (HidDevice d in allDevices)
        {
            try
            {
                string productName = d.GetProductName();

                if (productName.ToLower().Contains("muteme"))
                {
                    muteMeDevices.Add(d);
                }
            }
            catch (Exception dioe) when (dioe.GetType().Name.Contains("DeviceIOException"))
            {
                Console.WriteLine(dioe.Message);
            }
        }

        if (!muteMeDevices.Any())
        {
            return null;
        }

        HidDevice device = muteMeDevices[0];
        return new DeviceInfo(device.VendorID, device.ProductID);
    }

    public static bool DeviceExists(int vendorId, int productId)
    {
        return DeviceList.Local.TryGetHidDevice(out HidDevice device, vendorId, productId);
    }
}