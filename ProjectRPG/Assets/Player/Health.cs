using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RpgEssential
{
    public class BaseBar<T>{
        //The base of all bars with function definition and virtual function
        protected T maxAmount;
        protected T currentAmount;
    }
    public class BarContainer<T>: BaseBar<T>{
        //modular bar that could contain any type of contain (other than health)
    }
    //Do Health and an other modular bar class
    public class Health<T>: BaseBar<T>{
        public Health(T maxAmount, T currentAmount){
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;
        }
        public void SetmaxAmount(T amount){
            this.maxAmount = amount;
        }
        public void SetcurrentAmount(T amount){
            currentAmount = amount;
        }
        public T GetcurrentAmount(){
            return this.currentAmount;
        }
        public void TakeDamage(Health<int> health, int amount){
            if(health.GetcurrentAmount() > amount){
                health.SetcurrentAmount(health.GetcurrentAmount()-amount);
            }else
            {
                health.SetcurrentAmount(0);
            }
        }
    }
}
