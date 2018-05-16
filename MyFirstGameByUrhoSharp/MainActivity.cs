using Android.App;
using Android.Widget;
using Android.OS;
using Urho.Droid;
using Android.Views;
using Urho;
using Android.Hardware;
using System.Linq;
using Android.Runtime;
using Android.Content.PM;
using Android.Util;
using System.Collections.Generic;
using System;

namespace MyFirstGameByUrhoSharp
{
    [Activity(Label = "MyFirstGameByUrhoSharp", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity,ISensorEventListener
    {
        UrhoSurfacePlaceholder surface;
        MyApp app;
        private SensorManager mSensorManager;
        private Sensor mAccSensor,mMagneticSensor, mGeomagneticSensor;
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var mLayout = new FrameLayout(this);
            surface = UrhoSurface.CreateSurface(this);
            mLayout.AddView(surface);
            SetContentView(mLayout);
            app = await surface.Show<MyApp>(new ApplicationOptions("MyData"));


            mSensorManager = (SensorManager)GetSystemService(Activity.SensorService);
            mAccSensor = mSensorManager.GetDefaultSensor(SensorType.GeomagneticRotationVector);
            //mMagneticSensor = mSensorManager.GetDefaultSensor(SensorType.MagneticField);;
            mSensorManager.RegisterListener(this, mAccSensor, SensorDelay.Game);
           // mSensorManager.RegisterListener(this, mMagneticSensor, SensorDelay.Game);
        }

        protected override void OnResume()
        {
            UrhoSurface.OnResume();
            base.OnResume();
        }

        protected override void OnPause()
        {
            UrhoSurface.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory()
        {
            UrhoSurface.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnDestroy()
        {
            UrhoSurface.OnDestroy();
            mSensorManager.UnregisterListener(this);
            base.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Android.Views.Keycode.Back)
            {
                this.Finish();
                return false;
            }

            if (!UrhoSurface.DispatchKeyEvent(e))
                return false;
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }

        float[] data_acc = new float[3], data_mag = new float[3], gravity=new float[3];
        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.GeomagneticRotationVector)
            {
                var inR = new float[9];
                SensorManager.GetRotationMatrixFromVector(inR, e.Values.ToArray());
                var outR = new float[9];
                // we need to remap cooridante system, since the Y and Z axes will be swapped, when we pick up the device  
                if (SensorManager.RemapCoordinateSystem(inR, Android.Hardware.Axis.X, Android.Hardware.Axis.Z, outR))
                {
                    var ov = new float[3];
                    SensorManager.GetOrientation(outR, ov);
                    try
                    {
                        app.pitch = (MathHelper.RadiansToDegrees(ov[1]) + 360) % 360;
                        app.yaw = (MathHelper.RadiansToDegrees(ov[0]) + 360) % 360;
                        app.cameraNode.Rotation = new Quaternion(app.pitch, app.yaw, 0);
                    }
                    catch (System.Exception ex)
                    {
                        // while Urho.SimpleApplication is not fully started, the [app] properties are not available 
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }

                /* SensorType type = e.Sensor.Type;
             if (type == SensorType.Accelerometer) {
                 data_acc = e.Values.ToArray();
                 float alpha = 0.8f;

                 gravity[0] = alpha * gravity[0] + (1 - alpha) * data_acc[0];
                 gravity[1] = alpha * gravity[1] + (1 - alpha) * data_acc[1];
                 gravity[2] = alpha * gravity[2] + (1 - alpha) * data_acc[2];
                 data_acc[0] = data_acc[0] - gravity[0];
                 data_acc[1] = data_acc[1] - gravity[1];
                 data_acc[2] = data_acc[2] - gravity[2];
             }
             if (type == SensorType.MagneticField) {
                    data_mag = e.Values.ToArray();
             }

             if (data_acc!=null &&data_mag!=null) {
                 float[] R = new float[9];
                 float[] I = new float[9];

                 bool success = SensorManager.GetRotationMatrix(R, I, data_acc, data_mag);
                 if (success)
                 {
                     float[] orientation = new float[3];
                     SensorManager.GetOrientation(R, orientation);
                     app.pitch = (Urho.MathHelper.RadiansToDegrees(orientation[1]) + 360) % 360;      // map [-Pi...+Pi] to [0...360]
                     app.yaw = (Urho.MathHelper.RadiansToDegrees(orientation[0]) + 360) % 360;
                     Log.Error("pitch=", orientation[1] + "");
                     Log.Error("yaw=", orientation[0] + "");
                     app.cameraNode.Rotation = new Urho.Quaternion(app.pitch, app.yaw, 0);
                 }*/



             }
            
            
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            Log.Error("OnAccuracyChanged=", "OnAccuracyChanged");
        }
    }
}

