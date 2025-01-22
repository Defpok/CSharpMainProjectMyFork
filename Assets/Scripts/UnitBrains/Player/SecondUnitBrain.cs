using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        List<Vector2Int> DontReachTarget = new List<Vector2Int>();
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           
            
            ///////////////////////////////////////

            float curentTemperature = GetTemperature();

            if(curentTemperature >= overheatTemperature)
                return;
            IncreaseTemperature();

            for (int i = 0; i <= curentTemperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            
            



        }

        public override Vector2Int GetNextStep()
        {
            Vector2Int position = unit.Pos;
            Vector2Int nextPosition = new Vector2Int();
            Vector2Int target;

            if (DontReachTarget.Count > 0)
            {
                target = DontReachTarget[0];
            }
            else
            {
                return position;
            }

            if (IsTargetInRange(target))
            {
                return position;
            }
            else
            {
                nextPosition = target;
                return position.CalcNextStepTowards(nextPosition);
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = new List<Vector2Int>();
            var allTargets = GetAllTargets();

            float minDistance = float.MaxValue;
            Vector2Int nearestTarget = Vector2Int.zero;
            ///////////////////////////////////////
            if (allTargets != null)
            {
                foreach (Vector2Int target in GetAllTargets())
                {
                    if (DistanceToOwnBase(target) < minDistance)
                    {
                        minDistance = DistanceToOwnBase(target);
                        nearestTarget = target;
                    }
                }

                DontReachTarget.Clear();
                DontReachTarget.Add(nearestTarget);
                if (IsTargetInRange(nearestTarget))
                {
                    result.Add(nearestTarget);
                }
            }
            if (minDistance < float.MaxValue)
            {
                if (IsTargetInRange(nearestTarget))
                {
                    result.Add(nearestTarget);
                }
                else
                {
                    int playerID = IsPlayerUnitBrain ? RuntimeModel.PlayerId : RuntimeModel.BotPlayerId;
                    Vector2Int enemyBase = runtimeModel.RoMap.Bases[playerID];
                    DontReachTarget.Add(enemyBase);
                }
            }
            if (result.Count > 0)
            {
                result.Clear();
                result.Add(nearestTarget);
            }

            while (result.Count > 1)
            {
                result.RemoveAt(result.Count - 1);
            }


            return result;
        }  
            
        

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}