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

namespace MyFirstGameByUrhoSharp
{
    [Activity(Label = "MyFirstGameByUrhoSharp", MainLauncher = true, Icon = "@mipmap/icon",ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity,ISensorEventListener
    {
        UrhoSurfacePlaceholder surface;
        MyApp app;
        private SensorManager mSensorManager;
        private Sensor mRotationSensor;
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var mLayout = new FrameLayout(this);
            surface = UrhoSurface.CreateSurface(this);
            mLayout.AddView(surface);
            SetContentView(mLayout);
            app = await surface.Show<MyApp>(new ApplicationOptions("MyData"));


            mSensorManager = (SensorManager)GetSystemService(Activity.SensorService);
            mRotationSensor = mSensorManager.GetDefaultSensor(SensorType.RotationVector);
            mSensorManager.RegisterListener(this, mRotationSensor, SensorDelay.Game);
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


        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor == mRotationSensor)
            {
                var rm = new float[9];
                SensorManager.GetRotationMatrixFromVector(rm, e.Values.ToArray());
                var ov = new float[3];
                SensorManager.GetOrientation(rm, ov);
                app.pitch = (Urho.MathHelper.RadiansToDegrees(ov[1]) + 360) % 360;      // map [-Pi...+Pi] to [0...360]
                app.yaw = (Urho.MathHelper.RadiansToDegrees(ov[2]) + 360) % 360;
                Log.Error("pitch=",app.pitch+"");
                Log.Error("yaw=", app.yaw + "");
                // map [-Pi/2...+Pi/2] to [0...360]
                app.cameraNode.Rotation = new Urho.Quaternion(app.pitch, app.yaw, 0);

            }
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            Log.Error("OnAccuracyChanged=", "OnAccuracyChanged");
        }
    }
}

