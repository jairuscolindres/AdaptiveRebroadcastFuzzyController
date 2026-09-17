using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveRebroadcastFuzzyController
{
    internal class ARFC
    {
    }

    public struct SugenoResult
    {
        public double DecayFactor;
        public double Delay;

    }
    struct Packet
    {
        public int PacketId;
        public int DuplicateCount;
        public int FirstReceivedStep;
        public int NextCheckStep;
        public double CurrentProbability;
    }

    public class Node
    {
        public string Name {  get; set; }
        public int CurrentStep { get; set; } = 0;
        private Dictionary<int, Packet> trackedPackets = new Dictionary<int, Packet>();
        private HashSet<int> collectedPackets = new HashSet<int>();
        private Random rng = new Random();

        private const double ProbabilityFloor = 0.05;

        public Node(string name)
        {
            Name = name;
        }
            
        public void ReceivePacket(int packetId)
        {
            // 1. If we have ALREADY collected this packet in the past, do NOT track it as new!
            if (collectedPackets.Contains(packetId))
            {
                // If it's still actively being evaluated, just bump the duplicate counter
                if (trackedPackets.ContainsKey(packetId))
                {
                    var state = trackedPackets[packetId];
                    state.DuplicateCount++;
                    trackedPackets[packetId] = state;    
                }
                // If it was already purged from trackedPackets, IGNORE IT. Do not recreate it!
                return;
            }

            // 2. First time EVER seeing this packet
            collectedPackets.Add(packetId);
            trackedPackets[packetId] = new Packet
            {
                PacketId = packetId,
                DuplicateCount = 0,
                FirstReceivedStep = CurrentStep,
                CurrentProbability = 1.0,
                NextCheckStep = CurrentStep
            };
        }
        public List<int> RunStep()
        {
            var broadcastedThisStep = new List<int>();
            List<int> keys = new List<int>(trackedPackets.Keys);
            foreach(var packetId in keys)
            {
                var state = trackedPackets[packetId];
                if (CurrentStep < state.NextCheckStep)
                    continue;

                int elapsedSteps = CurrentStep - state.FirstReceivedStep;
                SugenoResult result = EvaluateBroadcastFuzzy(state.DuplicateCount, elapsedSteps);


                double roll = rng.NextDouble();
                if(roll < state.CurrentProbability)
                {
                    broadcastedThisStep.Add(packetId);
                }

                state.CurrentProbability *= result.DecayFactor;

                if(state.CurrentProbability < ProbabilityFloor)
                {
                    trackedPackets.Remove(packetId);
                    continue;
                }

                int delaySteps = Math.Max(1, (int)Math.Round(result.Delay));
                state.NextCheckStep = CurrentStep + delaySteps;
                trackedPackets[packetId] = state;
            }

            CurrentStep++;
            return broadcastedThisStep;
        }

        public bool IsTracking(int packetId) => trackedPackets.ContainsKey(packetId);

        public int TrackedPacketCount => trackedPackets.Count;

        public int CollectedPacketCount => collectedPackets.Count;

        private SugenoResult EvaluateBroadcastFuzzy(double duplicateCount, double elapsedTime)
        {
            double dupLow = TriangularMembership(duplicateCount, 0, 0, 2);
            double dupMed = TriangularMembership(duplicateCount, 1, 3, 5);
            double dupHigh = TriangularMembership(duplicateCount, 4, 8, 8);


            double timeNew = TriangularMembership(elapsedTime, 0, 0, 5);
            double timeAging = TriangularMembership(elapsedTime, 3, 10, 20);
            double timeOld = TriangularMembership(elapsedTime, 15, 30, 30);

            double r_LowNew = Math.Min(dupLow, timeNew);
            double r_LowAging = Math.Min(dupLow, timeAging);
            double r_LowOld = Math.Min(dupLow, timeOld);
            double r_MedNew = Math.Min(dupMed, timeNew);
            double r_MedAging = Math.Min(dupMed, timeAging);
            double r_MedOld = Math.Min(dupMed, timeOld);
            double r_HighNew = Math.Min(dupHigh, timeNew);
            double r_HighAging = Math.Min(dupHigh, timeAging);
            double r_HighOld = Math.Min(dupHigh, timeOld);

            double denom = r_LowNew + r_LowAging + r_LowOld +
                            r_MedNew + r_MedAging + r_MedOld +
                            r_HighNew + r_HighAging + r_HighOld;

            double cDecay_LowNew = 0.95;
            double cDecay_LowAging = 0.85;
            double cDecay_LowOld = 0.5;
            double cDecay_MedNew = 0.7;
            double cDecay_MedAging = 0.4;
            double cDecay_MedOld = 0.15;
            double cDecay_HighNew = 0.3;
            double cDecay_HighAging = 0.1;
            double cDecay_HighOld = 0.0;

            double cDelay_LowNew = 2.0;
            double cDelay_LowAging = 3.0;
            double cDelay_LowOld = 4.0;
            double cDelay_MedNew = 6.0;
            double cDelay_MedAging = 7.0;
            double cDelay_MedOld = 8.0;
            double cDelay_HighNew = 12.0;
            double cDelay_HighAging = 15.0;
            double cDelay_HighOld = 20.0;
//
            double numDecay = (r_LowNew * cDecay_LowNew)
                + (r_LowAging * cDecay_LowAging)
                + (r_LowOld * cDecay_LowOld)
                + (r_MedNew * cDecay_MedNew)
                + (r_MedAging * cDecay_MedAging)
                + (r_MedOld * cDecay_MedOld)
                + (r_HighNew * cDecay_HighNew)
                + (r_HighAging * cDecay_HighAging)
                + (r_HighOld * cDecay_HighOld);

            double numDelay = (r_LowNew * cDelay_LowNew)
                + (r_LowAging * cDelay_LowAging)
                + (r_LowOld * cDelay_LowOld)
                + (r_MedNew * cDelay_MedNew)
                + (r_MedAging * cDelay_MedAging)
                + (r_MedOld * cDelay_MedOld)
                + (r_HighNew * cDelay_HighNew)
                + (r_HighAging * cDelay_HighAging)
                + (r_HighOld * cDelay_HighOld);

            return new SugenoResult
            {
                DecayFactor = denom > 0 ? numDecay / denom : 0,
                Delay = denom > 0 ? numDelay / denom : 0
            };

        }

        private double TriangularMembership(double x, double a, double b, double c)
        {
            if (x <= a || x >= c) return 0.0;
            if (x == b) return 1.0;
            if (x > a && x < b) return (x - a) / (b - a);
            return (c - x) / (c - b);
        }
    }

    

}
