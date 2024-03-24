using System;
using System.Collections.Generic;

using Device = System.Int32;

namespace InputInterceptorNS
{

    public class DeviceData
    {

        public Device Device;
        public String CompositeName;
        public List<String> Names;

        public DeviceData(Device device, String rawCompositeName)
        {
            this.Device = device;
            this.CompositeName = String.Empty;
            this.Names = new List<String>();
            String[] names = rawCompositeName.Split('\0');
            foreach (String name in names)
            {
                if (name.Length > 0)
                {
                    this.CompositeName += name;
                    this.Names.Add(name);
                }
            }
        }

    }

}
