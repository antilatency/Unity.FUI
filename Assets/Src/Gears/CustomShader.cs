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
                    
                    //A very dirty way to update material parameters, all because Unity uses a material from its private cache for MaskableGraphic.
                    //When changing the base material, it will still use its own copy of the material from the cache, which prevents us from changing the parameters.
                    graphic.canvasRenderer.materialCount = 1;
                    graphic.canvasRenderer.SetMaterial(material, 0);
                }                
            }
        }

        public void SetParameters((string name, object value)[] parameters) {
            var graphic = GetComponent<Graphic>();
            
            //A very dirty way to update material parameters, all because Unity uses a material from its private cache for MaskableGraphic.
            //When changing the base material, it will still use its own copy of the material from the cache, which prevents us from changing the parameters.
            var material = graphic.canvasRenderer.GetMaterial();
            if (material == null){
                return;
            }

            foreach (var (name, value) in parameters) {
                if (value is Texture texture) {
                    material.SetTexture(name, texture);
                } else if (value is Vector4 vector4) {
                    material.SetVector(name, vector4);
                } else if (value is float valueAsFloat) {
                    material.SetFloat(name, valueAsFloat);
                } else if (value is bool valueAsBool) {
                    material.SetInt(name, valueAsBool ? 1 : 0);
                    if (valueAsBool) material.EnableKeyword(name);
                    else material.DisableKeyword(name);
                } else {
                    Debug.LogError($"CustomShader.SetParameters parameter type {value.GetType().FullName} is not supported.");
                }
            }
        }

    }

}