namespace dk.lego.devicesdk.device
{
    public enum IOType
    {
        /// NOTE: THESE ARE NOT USED DIRECTLY
        /**
         * A Motor - use the Motor to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.Motor}
         */
        IO_TYPE_MOTOR = 1,
        //0x01
        /**
         * A CIty train Motor - use Motor to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.Motor}
         */
        IO_TYPE_TRAIN_MOTOR = 2,  
        //0x02
        IO_TYPE_LIGHT = 8,
        /**
         * A Voltage Sensor - use the VoltageSensor to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.VoltageSensor}
         */
        IO_TYPE_VOLTAGE = 20,
        //0x14
        /**
         * A Current Sensor - use the CurrentSensor to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.CurrentSensor}
         */
        IO_TYPE_CURRENT = 21,
        //0x15
        /**
         * A Piezo Tone player - use the PiezoTonePlayer to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.PiezoTonePlayer}
         */
        IO_TYPE_PIEZO_TONE_PLAYER = 22,
        //0x16
        /**
         * An RGB light - use the RGBLight to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.RGBLight}
         */
        IO_TYPE_RGB_LIGHT = 23,
        //0x17
        /**
         * A Tilt Sensor - use the TiltSensor to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.TiltSensor}
         */
        IO_TYPE_TILT_SENSOR = 34,
        //0x22
        /**
         * A Motion Sensor  = aka. Detect Sensor) - use the MotionSensor to communicate with this type of IO.
         * See {@link dk.lego.devicesdk.services.MotionSensor}
         */
        IO_TYPE_MOTION_SENSOR = 35,
        //0x23
        /**
         * A Vision Sensor - use the VisionSensor to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.VisionSensor}
         */
        IO_TYPE_VISION_SENSOR = 37,
        //0x0025
        /**
         * A Motor /w Tacho - use the MotorWithTacho to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.MotorWithTacho}
         */
        IO_TYPE_MOTOR_WITH_TACHO = 38,
        //0x0026
        /**
         * An internal Motor /w Tacho - use the MotorWithTacho to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.MotorWithTacho}
         */
        IO_TYPE_INTERNAL_MOTOR_WITH_TACHO = 39,
        //0x0027
        /**
         * An internal 3-axis Tilt Sensor - use the TiltSensorThreeAxis to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.TiltSensorThreeAxis}
         */
        IO_TYPE_INTERNAL_TILT_SENSOR_THREE_AXIS = 40,
        //0x0028
        /**
         * A Motor with PVM Signal, use the Motor to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.Motor}
         */
        IO_TYPE_DT_MOTOR = 41,
        //0x0029
        /**
         * A Sound player - use the SoundPlayer to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.SoundPlayer}
         */
        IO_TYPE_SOUND_PLAYER = 42,
        //0x002a
        /**
         * A Color sensor - use the ColorSensor to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.ColorSensor}
         */
        IO_TYPE_COLOR_SENSOR = 43,
        //0x002b
        /**
         * A Move sensor - use the MoveSensor to communicate with this type of IO
         * See {@link dk.lego.devicesdk.services.MoveSensor}
         */
        IO_TYPE_MOVE_SENSOR = 44,
        IO_TYPE_TECHNIC_MOTOR_LARGE = 46,
        IO_TYPE_TECHNIC_MOTOR_EXTRA_LARGE = 47,
        IO_TYPE_TECHNIC_AZURE_ANGULAR_MOTOR_MEDIUM = 48,
        IO_TYPE_TECHNIC_AZURE_ANGULAR_MOTOR_LARGE = 49,
        IO_TYPE_TECHNIC_3_AXIS_GESTURE = 54,
        IO_TYPE_REMOTE_CONTROL_BUTTON_SENSOR = 55,
        IO_TYPE_TECHNIC_3_AXIS_ACCELEROMETER = 57,
        IO_TYPE_TECHNIC_3_AXIS_GYRO_SENSOR = 58,
        IO_TYPE_TECHNIC_3_AXIS_ORIENTATION_SENSOR = 59,
        IO_TYPE_TECHNIC_TEMPERATURE_SENSOR = 60,
        IO_TYPE_TECHNIC_COLOR_SENSOR = 61,
        IO_TYPE_TECHNIC_DISTANCE_SENSOR = 62,
        IO_TYPE_TECHNIC_FORCE_SENSOR = 63,
        IO_TYPE_GECKO_LED_MATRIX = 64,
        IO_TYPE_TECHNIC_AZURE_ANGULAR_MOTOR_SMALL = 65,
        IO_TYPE_BOOST_VM = 66,
        
        //0x002c
        /**
         * A type unknown to the SDK - use the {@link dk.lego.devicesdk.services.GenericService} to communicate with this type of IO.
         */
        IO_TYPE_GENERIC = 0
    }
}
