using System.Collections.Generic;

namespace EEMod.MachineLearning
{
    public class NeuronInterface
    {
        public List<float> finalLayer = new List<float>();

        public List<List<float>> finalLayerHolder = new List<List<float>>();

        public List<float> answerLayer = new List<float>();

        public List<List<float>> answerHolder = new List<List<float>>();

        public List<float> errors = new List<float>();
    }
}