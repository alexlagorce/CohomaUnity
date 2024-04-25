//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.UnityRoboticsDemo
{
    [Serializable]
    public class TelemetryMsg : Message
    {
        public const string k_RosMessageName = "unity_robotics_demo_msgs/Telemetry";
        public override string RosMessageName => k_RosMessageName;

        //  Telemetry Message Definition
        public float battery_percentage;
        public float battery_voltage;
        public float latitude;
        public float longitude;
        public float altitude;

        public TelemetryMsg()
        {
            this.battery_percentage = 0.0f;
            this.battery_voltage = 0.0f;
            this.latitude = 0.0f;
            this.longitude = 0.0f;
            this.altitude = 0.0f;
        }

        public TelemetryMsg(float battery_percentage, float battery_voltage, float latitude, float longitude, float altitude)
        {
            this.battery_percentage = battery_percentage;
            this.battery_voltage = battery_voltage;
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;
        }

        public static TelemetryMsg Deserialize(MessageDeserializer deserializer) => new TelemetryMsg(deserializer);

        private TelemetryMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.battery_percentage);
            deserializer.Read(out this.battery_voltage);
            deserializer.Read(out this.latitude);
            deserializer.Read(out this.longitude);
            deserializer.Read(out this.altitude);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.battery_percentage);
            serializer.Write(this.battery_voltage);
            serializer.Write(this.latitude);
            serializer.Write(this.longitude);
            serializer.Write(this.altitude);
        }

        public override string ToString()
        {
            return "TelemetryMsg: " +
            "\nbattery_percentage: " + battery_percentage.ToString() +
            "\nbattery_voltage: " + battery_voltage.ToString() +
            "\nlatitude: " + latitude.ToString() +
            "\nlongitude: " + longitude.ToString() +
            "\naltitude: " + altitude.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
