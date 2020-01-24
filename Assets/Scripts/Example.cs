using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues( typeof( Joycon.Button ) ) as Joycon.Button[];

    private List<Joycon>    m_joycons;
    private Joycon          m_joyconL;
    private Joycon          m_joyconR;
    private Joycon.Button?  m_pressedButtonL;
    private Joycon.Button?  m_pressedButtonR;

    [SerializeField]
    [Range(0.0f, 500.0f)]
    float low_freq;
    [SerializeField]
    [Range(0.0f, 500.0f)]
    float high_freq;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float amp;

    [SerializeField]
    Slider gyroX;
    [SerializeField]
    Slider gyroY;
    [SerializeField]
    Slider gyroZ;

    [SerializeField]
    Slider accelX;
    [SerializeField]
    Slider accelY;
    [SerializeField]
    Slider accelZ;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if ( m_joycons == null || m_joycons.Count <= 0 ) return;

        m_joyconL = m_joycons.Find( c =>  c.isLeft );
        m_joyconR = m_joycons.Find( c => !c.isLeft );
    }

    private void Update()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if ( m_joycons == null || m_joycons.Count <= 0 ) return;

        foreach ( var button in m_buttons )
        {
            if ( m_joyconL.GetButton( button ) )
            {
                m_pressedButtonL = button;
            }
            if ( m_joyconR.GetButton( button ) )
            {
                m_pressedButtonR = button;
            }
        }

        if ( Input.GetKey( KeyCode.Z ) )
        {
            m_joyconL.SetRumble( this.low_freq, this.high_freq, this.amp, 15 );
        }
        if ( Input.GetKey( KeyCode.X ) )
        {
            m_joyconR.SetRumble( this.low_freq, this.high_freq, this.amp, 15 );
        }

        this.gyroX.value = Mathf.Abs(this.m_joyconL.GetGyro().x);
        this.gyroY.value = Mathf.Abs(this.m_joyconL.GetGyro().y);
        this.gyroZ.value = Mathf.Abs(this.m_joyconL.GetGyro().z);

        this.accelX.value = Mathf.Abs(this.m_joyconL.GetAccel().x);
        this.accelY.value = Mathf.Abs(this.m_joyconL.GetAccel().y);
        this.accelZ.value = Mathf.Abs(this.m_joyconL.GetAccel().z);
    }

    private void OnGUI()
    {
        var style = GUI.skin.GetStyle( "label" );
        style.fontSize = 24;

        if ( m_joycons == null || m_joycons.Count <= 0 )
        {
            GUILayout.Label( "Joy-Con が接続されていません" );
            return;
        }

        if ( !m_joycons.Any( c => c.isLeft ) )
        {
            GUILayout.Label( "Joy-Con (L) が接続されていません" );
            return;
        }

        if ( !m_joycons.Any( c => !c.isLeft ) )
        {
            GUILayout.Label( "Joy-Con (R) が接続されていません" );
            return;
        }

        GUILayout.BeginHorizontal( GUILayout.Width( 960 ) );

        foreach ( var joycon in m_joycons )
        {
            var isLeft      = joycon.isLeft;
            var name        = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key         = isLeft ? "Z キー" : "X キー";
            var button      = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick       = joycon.GetStick();
            var gyro        = joycon.GetGyro();
            var accel       = joycon.GetAccel();
            var orientation = joycon.GetVector();

            GUILayout.BeginVertical( GUILayout.Width( 480 ) );
            GUILayout.Label( name );
            GUILayout.Label( key + "：振動" );
            GUILayout.Label( "押されているボタン：" + button );
            GUILayout.Label( string.Format( "スティック：({0}, {1})", stick[ 0 ], stick[ 1 ] ) );
            GUILayout.Label( "ジャイロ：" + gyro );
            GUILayout.Label( "加速度：" + accel );
            GUILayout.Label( "傾き：" + orientation );
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
    }
}