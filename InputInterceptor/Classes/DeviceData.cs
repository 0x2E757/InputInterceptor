using System;

using Device = System.Int32;

namespace InputInterceptorNS {

    public class DeviceData {

        public Device Device;
        public String Name;

        public DeviceData(Device device, String name) {
            this.Device = device;
            this.Name = name;
        }

    }

}
