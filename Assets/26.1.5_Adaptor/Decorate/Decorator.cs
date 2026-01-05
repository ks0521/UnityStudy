using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//데코레이터 패턴 : 기존 객체에 추가적인 요소를 더해주는 패턴
namespace Decorate
{
    public interface IGun
    {
        public float GetNoise(); //격발음
        public float GetVirticalSway(); //수직반동
        public float GetHorizontalSway(); //수평반동
    }
    public class GunPart : IGun
    {
        IGun gun;
        public GunPart(IGun gun)
        {
            this.gun = gun;
        }
        public virtual float GetHorizontalSway()
        {
            return gun.GetHorizontalSway();
        }

        public virtual float GetNoise()
        {
            return gun.GetNoise();
        }

        public virtual float GetVirticalSway()
        {
            return gun.GetVirticalSway();
        }
    }
    public class VirticalGrip : GunPart
    {
        float reduceValue;
        public VirticalGrip(IGun gun) : base(gun) 
        {
            reduceValue = 1;
        }
        public override float GetVirticalSway()
        {
            return base.GetVirticalSway() - reduceValue;
        }
        public override float GetHorizontalSway()
        {
            return base.GetHorizontalSway() + reduceValue / 2;
        }
    }
    public class Muzzle : GunPart
    {
        public float reduceValue;
        public Muzzle(IGun gun) : base(gun)
        {
            reduceValue = 1;
        }
        public override float GetNoise()
        {
            return base.GetNoise() - reduceValue;
        }

    }
    public class Pistol : IGun
    {
        public float noise;
        public float virticalSway;
        public float horizontalSway;

        public Pistol()
        {
            noise = 5.0f;
            virticalSway = 1.0f;
            horizontalSway = 1.0f;
        }
        public float GetNoise()
        {
            return noise;   
        }
        public float GetHorizontalSway()
        {
            return horizontalSway;
        }
        public float GetVirticalSway()
        {
            return virticalSway;
        }

    }
    public class Decorator : MonoBehaviour
    {
        IGun pistol;
        void Start()
        {
            pistol = new Pistol();
            Debug.Log(pistol.GetNoise());
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(pistol.GetVirticalSway());
                Debug.Log(pistol.GetNoise());
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                pistol = new VirticalGrip(pistol);
                Debug.Log(pistol.GetVirticalSway());
                Debug.Log(pistol.GetNoise());
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                pistol = new Muzzle(pistol);
                Debug.Log(pistol.GetVirticalSway());
                Debug.Log(pistol.GetNoise());
            }
        }
    }

}
