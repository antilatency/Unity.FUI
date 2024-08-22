using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FUI.Gears {
    public class CustomShader : MonoBehaviour {
        private string shaderName;

        public string ShaderName {
            
            get => shaderName;

            set {
                if (shaderName != value) {
                    shaderName = value;
                    var shader = Shader.Find(shaderName);
                    Material material = new Material(shader);
                    GetComponent<Graphic>().material = material;
                }                
            }
        }
    }

}