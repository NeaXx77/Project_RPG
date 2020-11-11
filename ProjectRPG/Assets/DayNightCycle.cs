using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DayNightCycle : MonoBehaviour
{
    //with timeHour, and curve get the curve value,
    // lerp light color with the curve return value
    public AnimationCurve TransitionCurve,temporisationCurve;
    [SerializeField]
    static float timeHour;
    public Color dawn;
    public Color morning;
    public Color evening;
    public Color dusk;
    public Color night;
    enum DayPeriod {
        isDawn,
        isMorning,
        isEvening,
        isDusk,
        isNight
    };
    DayPeriod dayPeriod;
    public Light sun;
    [SerializeField]
    //timeForADay in minute
    float timeForADay, actualTime;
    //to modify
    bool isDay = true;
    public float startHour = 6;
    public Text dayPeriodText;
    private void Awake() {
        //While save not scripted
        dayPeriod = DayPeriod.isDawn;
        timeForADay /= 6;
        timeHour = DeterminedayPeriod(startHour);
        print(timeForADay);
    }
    private void Update() {
        float deltaUpdateTime = Time.deltaTime;
        if (dayPeriod == DayPeriod.isDawn){
            sun.color = Color.Lerp(dawn,morning,timeHour);
            if(isTransitionFinished(sun.color,morning)){
                dayPeriod++;
                print("10h - "+actualTime);
            }
        }else if (dayPeriod == DayPeriod.isMorning){
            sun.color = Color.Lerp(morning,evening,timeHour);
            if(isTransitionFinished(sun.color,evening)){
                dayPeriod++;
                print("14 - "+actualTime);
            }
        }else if(dayPeriod == DayPeriod.isEvening){
            sun.color = Color.Lerp(evening,dusk,timeHour);
            if(isTransitionFinished(sun.color,dusk)){
                dayPeriod++;
                isDay = false;
                print("18 - "+actualTime);
            }
        }else if(dayPeriod == DayPeriod.isDusk){
            sun.color = Color.Lerp(dusk,night,timeHour);
            if(isTransitionFinished(sun.color,night)){
                dayPeriod++;
                //Midnight reset Hour
                actualTime = 0;
                print("00 - "+actualTime);
            }
        }else if(dayPeriod == DayPeriod.isNight){
            sun.color = Color.Lerp(night,dawn,timeHour);
            if(isTransitionFinished(sun.color,dawn)){
                dayPeriod = 0;
                isDay = true;
                print("6 - "+actualTime);
            }
        }
        if(isDay){
            UpdateTimeDay();
        }else{
            UpdateTimeNight();
        }
        dayPeriodText.text = ToString();
    }
    bool isTransitionFinished(Color toCompare, Color light){
        if(toCompare == light){
            timeHour = 0;
            return true;
        }
        return false;
    }
    void UpdateTimeDay(){
        float timeFormula = Time.deltaTime/(timeForADay*60);
        timeHour += timeFormula;
        actualTime += timeFormula *4;//multiply by number of hour in the split
    }
    void UpdateTimeNight(){
        //90 because have to add 0.5 of the normal time,
        //normal time is 4h, night is split by 6h
        float timeFormula = Time.deltaTime/(timeForADay*60*1.5f);
        timeHour += timeFormula;
        actualTime += timeFormula *6;
    }
    public float DeterminedayPeriod(float startHour){
        if(startHour > 24){
            startHour -= 24;
        }else if(startHour < 0){
            startHour = 0;
        }

        actualTime = startHour;

        if(startHour >= 6 && startHour < 10){
            dayPeriod = DayPeriod.isDawn;
            return (startHour - 6)/4;
        }else if(startHour >= 10 && startHour < 14){
            dayPeriod = DayPeriod.isMorning;
            return (startHour - 10)/4;
        }else if(startHour >= 14 && startHour < 18){
            dayPeriod = DayPeriod.isEvening;
            return (startHour - 14)/4;
        }else if(startHour >= 18 && startHour <= 23.99f){
            dayPeriod = DayPeriod.isDusk;
            return (startHour - 18)/6;
        }else if(startHour >= 0 && startHour < 6){
            dayPeriod = DayPeriod.isNight;
            return (startHour)/6;
        }
        return 0;
    }

    public override string ToString(){
        int dayHour = (int)actualTime;
        float minute = actualTime - dayHour;
        minute *= 60;
        minute = (int)minute;
        return (minute < 10)?(dayHour+" : 0"+minute):(dayHour+" : "+minute);
    }
}
