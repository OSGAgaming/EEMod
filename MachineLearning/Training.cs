using System.Collections.Generic;

namespace EEMod.MachineLearning
{
    public class Trainer
    {
        public double[] inputs;
        public float[] kerneledInputs;
        public float[] answer;
        public List<float> term = new List<float>();
        public double[] input;
        public int column;

        public Trainer(double[] input, float[] a, int column)
        {
            this.input = input;
            this.column = column;
            answer = a;
        }
    }
}