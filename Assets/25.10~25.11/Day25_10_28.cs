using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day25_10_28 : MonoBehaviour
{
    enum Day
    {
        Monday = 0,
        Tuesday = 1, 
        Wednesday = 2,
        Thursday = 3, 
        Friday = 4,
        Saturday = 5,
        Sunday = 6
    }
    struct EventDay
    {
        private Day day;
        private int hour;

        public EventDay(Day day, int hour)
        {
            this.day = day;
            this.hour = hour;
        }
        private void SetHour(int hour)
        {
            this.hour += hour;
            while (this.hour >= 24)
            {
                SetDay(1);
                this.hour -= 24;
            }
        }
        private void SetDay(int day)
        {
            if(day<0)
            {
                Debug.LogError($"Day Input Error: {day}");
                return;
            }
            this.day = (Day) ( ( (int)this.day + day ) % 7); //7일 단위로 요일이 반복
            
        }
        public int GetHour()
        {
            return this.hour;
        }
        public Day GetDay()
        {
            return this.day;
        }
        public void NextEvent(int day, int hour) //day 일 hour시간 후 다시 이벤트가 시작되는 시간 안내
        {
            SetHour(hour);
            SetDay(day);
            Debug.Log($"Next Event is {this.day}, {this.hour} hour!");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        EventDay event1 = new EventDay(Day.Monday, 4);
        for(int i = 0; i < 6; i++)
        {
            Debug.Log($"Today is {event1.GetDay()}, {event1.GetHour()} hour!");
            event1.NextEvent(1, 4);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
