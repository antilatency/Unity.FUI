using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FUI.Gears {
    public class CustomShader : MonoBehaviour {
        [SerializeField]
        private string shaderName;


        public string ShaderName {
            
            get => shaderName;

            set {
                if (shaderName != value) {
                    shaderName = value;
                    var shader = Shader.Find(shaderName);
                    Material material = new Material(shader);
                    var graphic = GetComponent<Graphic>();
                    graphic.material = material;
                }                
            }
        }

        public void SetParameters((string name, object value)[] parameters) {
            var graphic = GetComponent<Graphic>();
            var material = graphic.material;

            foreach (var (name, value) in parameters) {
                if (value is Texture texture) {
                    material.SetTexture(name, texture);
                } else if (value is Vector4 vector4) {
                    material.SetVector(name, vector4);
                } else if (value is bool valueAsBool) {
                    material.SetInt(name, valueAsBool ? 1 : 0);
                    if (valueAsBool) material.EnableKeyword(name);
                    else material.DisableKeyword(name);
                }
            }
        }

    }

}