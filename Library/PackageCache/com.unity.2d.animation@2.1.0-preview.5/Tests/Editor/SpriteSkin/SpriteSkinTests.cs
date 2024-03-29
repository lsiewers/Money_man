using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.Experimental.U2D.Common;
using Unity.Jobs;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace UnityEditor.Experimental.U2D.Animation.Test.Skinning
{

    public class SpriteSkinTests
    {
        private class Vector3Compare : IEqualityComparer<Vector3>
        {
            public bool Equals(Vector3 a, Vector3 b)
            {
                return Vector3.Distance(a, b) < Epsilon;
            }

            public int GetHashCode(Vector3 v)
            {
                return v.GetHashCode();
            }

            private static readonly float Epsilon = 0.001f;
        }

        private class Float3Compare : IEqualityComparer<float3>
        {
            public bool Equals(float3 a, float3 b)
            {
                return math.distance(a, b) < Epsilon;
            }

            public int GetHashCode(float3 v)
            {
                return v.GetHashCode();
            }

            private static readonly float Epsilon = 0.001f;
        }

        private class QuaternionCompare : IEqualityComparer<Quaternion>
        {
            public bool Equals(Quaternion a, Quaternion b)
            {
                return Quaternion.Dot(a, b) > 1f - Epsilon;
            }

            public int GetHashCode(Quaternion v)
            {
                return v.GetHashCode();
            }

            private static readonly float Epsilon = 0.001f;
        }

        private readonly float3[] kRootRotateDeformedVertices =
        {
            new float3(-1.010294f, 2.572992f, 0f),
            new float3(-1.030294f, 2.752992f, 0f),
            new float3(-1.090294f, 2.752992f, 0f),
            new float3(-1.360294f, 2.202992f, 0f),
            new float3(-1.540294f, 2.112992f, 0f),
            new float3(-1.740294f, 1.982992f, 0f),
            new float3(-1.750294f, 1.952992f, 0f),
            new float3(-1.750294f, 1.792992f, 0f),
            new float3(-1.680294f, 1.802992f, 0f),
            new float3(-1.690294f, 1.592992f, 0f),
            new float3(-1.650294f, 1.572992f, 0f),
            new float3(-1.630294f, 1.572992f, 0f),
            new float3(-1.580294f, 1.652992f, 0f),
            new float3(-1.580294f, 1.282992f, 0f),
            new float3(-1.750294f, 1.112992f, 0f),
            new float3(-1.750294f, 1.042992f, 0f),
            new float3(-1.700294f, 0.972992f, 0f),
            new float3(-1.180294f, 0.802992f, 0f),
            new float3(-1.220294f, 0.5829921f, 0f),
            new float3(-1.550294f, 0.382992f, 0f),
            new float3(-1.550294f, 0.3429921f, 0f),
            new float3(-1.220294f, 0.362992f, 0f),
            new float3(-1.310294f, 0.2429921f, 0f),
            new float3(-1.340294f, 0.1929921f, 0f),
            new float3(-1.050294f, 0.1929921f, 0f),
            new float3(-0.970294f, 0.2029921f, 0f),
            new float3(-0.750294f, 0.472992f, 0f),
            new float3(-0.7502939f, 0.822992f, 0f),
            new float3(-0.5502939f, 0.992992f, 0f),
            new float3(-0.450294f, 1.222992f, 0f),
            new float3(-0.4502939f, 1.682992f, 0f),
            new float3(-0.530294f, 1.932992f, 0f),
            new float3(-0.6202939f, 2.072992f, 0f),
            new float3(-0.9102941f, 2.462992f, 0f)
        };

        private readonly float3[] kChildRotateDeformedVertices =
        {
            new float3(-0.6816357f, 1.782048f, 0f),
            new float3(-0.7016357f, 1.962048f, 0f),
            new float3(-0.7616357f, 1.962048f, 0f),
            new float3(-1.031636f, 1.412048f, 0f),
            new float3(-1.211636f, 1.322048f, 0f),
            new float3(-1.411636f, 1.192048f, 0f),
            new float3(-1.421636f, 1.162048f, 0f),
            new float3(-1.421636f, 1.002048f, 0f),
            new float3(-1.351636f, 1.012048f, 0f),
            new float3(-1.361636f, 0.8020483f, 0f),
            new float3(-1.321636f, 0.7820483f, 0f),
            new float3(-1.301636f, 0.7820483f, 0f),
            new float3(-1.251636f, 0.8620483f, 0f),
            new float3(-1.251636f, 0.4920483f, 0f),
            new float3(-1.421636f, 0.3220483f, 0f),
            new float3(-1.421636f, 0.2520483f, 0f),
            new float3(-1.371636f, 0.1820483f, 0f),
            new float3(-0.84188f, 0.02294878f, 0f),
            new float3(-0.890622f, 0.07894892f, 0f),
            new float3(-1.09f, 0.585f, 0f),
            new float3(-1.13f, 0.585f, 0f),
            new float3(-1.097388f, 0.2155563f, 0f),
            new float3(-1.23f, 0.345f, 0f),
            new float3(-1.28f, 0.375f, 0f),
            new float3(-1.228463f, 0.02196335f, 0f),
            new float3(-1.178291f, -0.08154059f, 0f),
            new float3(-0.6511877f, -0.2770902f, 0f),
            new float3(-0.4241534f, 0.02932463f, 0f),
            new float3(-0.2216356f, 0.2020484f, 0f),
            new float3(-0.1216356f, 0.4320484f, 0f),
            new float3(-0.1216356f, 0.8920484f, 0f),
            new float3(-0.2016356f, 1.142048f, 0f),
            new float3(-0.2916357f, 1.282048f, 0f),
            new float3(-0.5816357f, 1.672048f, 0f)
        };

        private SpriteSkin m_SpriteSkin;

        private Sprite riggedSprite;
        private Sprite staticSprite;

        private Vector3Compare vec3Compare = new Vector3Compare();
        private Float3Compare float3Compare = new Float3Compare();
        private QuaternionCompare quatCompare = new QuaternionCompare();

        [SetUp]
        public void Setup()
        {
            riggedSprite = Resources.Load<Sprite>("bird");
            staticSprite = Resources.Load<Sprite>("star");
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(riggedSprite));
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(staticSprite));

            m_SpriteSkin = new GameObject("TestObject1").AddComponent<SpriteSkin>();
            m_SpriteSkin.spriteRenderer.sprite = riggedSprite;
            m_SpriteSkin.CreateBoneHierarchy();
        }

        [TearDown]
        public void Teardown()
        {
            GameObject.DestroyImmediate(m_SpriteSkin.gameObject);
        }

        [Test]
        public void CreateSkeleton_CreatesValidSkeletonFromBones()
        {
            Assert.AreEqual(1, m_SpriteSkin.transform.childCount);
            var transforms = m_SpriteSkin.transform.GetComponentsInChildren<Transform>();
            Assert.AreEqual(4, transforms.Length); // GameObject + 3 Bones

            Assert.AreEqual("root", transforms[1].gameObject.name);
            Assert.That(new Vector3(-1.219143f, 0.253849f, 0.0f), Is.EqualTo(transforms[1].position).Using(vec3Compare));
            Assert.That(new Quaternion(0f, 0f, -0.1945351f, 0.9808956f), Is.EqualTo(transforms[1].rotation).Using(quatCompare));
            Assert.AreEqual("bone_HGS", transforms[2].gameObject.name);
            Assert.That(new Vector3(-0.659342f, 0.0227064f, 0f), Is.EqualTo(transforms[2].position).Using(vec3Compare));
            Assert.That(new Quaternion(0f, 0f, 0.01651449f, 0.9998637f), Is.EqualTo(transforms[2].rotation).Using(quatCompare));
            Assert.AreEqual("bone_HGS_KTR", transforms[3].gameObject.name);
            Assert.That(new Vector3(0.7444712f, 0.08755364f, 0.0f), Is.EqualTo(transforms[3].position).Using(vec3Compare));
            Assert.That(new Quaternion(0f, 0f, 0.003135923f, 0.9999951f), Is.EqualTo(transforms[3].rotation).Using(quatCompare));
        }

        [Test]
        public void CreateBoneHierarchy_MakesSpriteSkinValid()
        {
            Assert.IsTrue(m_SpriteSkin.isValid);
        }

        [Test]
        public void SpriteWithNoSkinningData_IsNotValid()
        {
            m_SpriteSkin.spriteRenderer.sprite = staticSprite;
            Assert.IsFalse(m_SpriteSkin.isValid);
        }

        [Test]
        public void ValidateWithNoSprite_ReturnsSpriteNotFoundResult()
        {
            m_SpriteSkin.spriteRenderer.sprite = null;
            Assert.AreEqual(SpriteSkinValidationResult.SpriteNotFound, m_SpriteSkin.Validate());
        }

        [Test]
        public void ValidateWithNoBindPoses_ReturnsSpriteHasNoSkinningInformation()
        {
            m_SpriteSkin.spriteRenderer.sprite = staticSprite;
            Assert.AreEqual(SpriteSkinValidationResult.SpriteHasNoSkinningInformation, m_SpriteSkin.Validate());
        }

        [Test]
        public void ValidateWithNoRootBone_ReturnsRootTransformNotFound()
        {
            m_SpriteSkin.rootBone = null;
            Assert.AreEqual(SpriteSkinValidationResult.RootTransformNotFound, m_SpriteSkin.Validate());
        }

        [Test]
        public void ValidateWithNullBoneTransformArray_ReturnsInvalidTransformArray()
        {
            m_SpriteSkin.boneTransforms = null;
            Assert.AreEqual(SpriteSkinValidationResult.InvalidTransformArray, m_SpriteSkin.Validate());
        }

        [Test]
        public void ValidateWithIncorrectBoneTransformArrayLength_ReturnsInvalidTransformArrayLength()
        {
            m_SpriteSkin.boneTransforms = new Transform[5];
            Assert.AreEqual(SpriteSkinValidationResult.InvalidTransformArrayLength, m_SpriteSkin.Validate());
        }

        [Test]
        public void ValidateWithNullRefsInBoneTransformArray_ReturnsTransformArrayContainsNull()
        {
            m_SpriteSkin.boneTransforms = new Transform[3];
            Assert.AreEqual(SpriteSkinValidationResult.TransformArrayContainsNull, m_SpriteSkin.Validate());
        }

        [Test]
        public void ValidateWithNoRootBoneInBoneTransformArray_ReturnsRootNotFoundInTransformArray()
        {
            var aTransform = new GameObject().transform;

            m_SpriteSkin.boneTransforms[0] = aTransform;
            Assert.AreEqual(SpriteSkinValidationResult.RootNotFoundInTransformArray, m_SpriteSkin.Validate());

            GameObject.DestroyImmediate(aTransform.gameObject);
        }

        [Test]
        public void ResetBindPose_HasNoEffectOnRoot()
        {
            var localRotation = m_SpriteSkin.rootBone.localRotation;
            m_SpriteSkin.rootBone.localRotation = Quaternion.Euler(0f, 0f, 32f);
            Assert.AreNotEqual(localRotation, m_SpriteSkin.rootBone.localRotation);
            m_SpriteSkin.ResetBindPose();
            Assert.AreNotEqual(localRotation, m_SpriteSkin.rootBone.localRotation);
        }

        [Test]
        public void ResetBindPose_RestoresTransformPositionRotationAndScale()
        {
            var localRotation = m_SpriteSkin.boneTransforms[1].localRotation;
            var localPosition = m_SpriteSkin.boneTransforms[1].localPosition;
            var localScale = m_SpriteSkin.boneTransforms[1].localScale;
            m_SpriteSkin.boneTransforms[1].localRotation = Quaternion.Euler(0f, 0f, 32f);
            m_SpriteSkin.boneTransforms[1].localPosition += Vector3.one * 10;
            m_SpriteSkin.boneTransforms[1].localScale += Vector3.one * 10;
            Assert.AreNotEqual(localRotation, m_SpriteSkin.boneTransforms[1].localRotation);
            Assert.AreNotEqual(localPosition, m_SpriteSkin.boneTransforms[1].localPosition);
            Assert.AreNotEqual(localScale, m_SpriteSkin.boneTransforms[1].localScale);
            m_SpriteSkin.ResetBindPose();
            Assert.AreEqual(localRotation, m_SpriteSkin.boneTransforms[1].localRotation);
            Assert.AreEqual(localPosition, m_SpriteSkin.boneTransforms[1].localPosition);
            Assert.AreEqual(localScale, m_SpriteSkin.boneTransforms[1].localScale);
        }

        private void DeformJob(SpriteSkin spriteSkin, NativeArray<Vector3> deformableVertices)
        {
            SpriteSkinUtility.Deform(spriteSkin.spriteRenderer.sprite, spriteSkin.transform.worldToLocalMatrix, m_SpriteSkin.boneTransforms, ref deformableVertices);
        }

        private void AssertDeformation(SpriteSkin spriteSkin, float3[] expectedVertices)
        {
            var deformedVertices = new NativeArray<Vector3>(spriteSkin.spriteRenderer.sprite.GetVertexCount(), Allocator.Temp);

            DeformJob(spriteSkin, deformedVertices);

            for (var i = 0; i < deformedVertices.Length; ++i)
            {
                var v3 = deformedVertices[i];
                var f3 = new float3(v3.x, v3.y, v3.z);
                Assert.That(f3, Is.EqualTo(expectedVertices[i]).Using(float3Compare));
            }

            deformedVertices.Dispose();
        }

        [Test]
        public void RotateRoot_ProducesExpectedDeformation()
        {
            m_SpriteSkin.rootBone.Rotate(new Vector3(0, 0, 90.0f));

            AssertDeformation(m_SpriteSkin, kRootRotateDeformedVertices);
        }

        [Test]
        public void RotateChild_ProducesExpectedDeformation()
        {
            m_SpriteSkin.boneTransforms[1].Rotate(new Vector3(0, 0, 90.0f));

            AssertDeformation(m_SpriteSkin, kChildRotateDeformedVertices);
        }

        [Test]
        public void RotateTransform_ChangesHash()
        {
            int hash = m_SpriteSkin.CalculateTransformHash();
            m_SpriteSkin.boneTransforms[1].Rotate(new Vector3(0, 0, 90.0f));
            Assert.That(hash, !Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
            m_SpriteSkin.ResetBindPose();
            Assert.That(hash, Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
            m_SpriteSkin.transform.Rotate(new Vector3(0, 0, 90.0f));
            Assert.That(hash, !Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
        }

        [Test]
        public void MoveTransform_ChangesHash()
        {
            int hash = m_SpriteSkin.CalculateTransformHash();
            m_SpriteSkin.boneTransforms[1].position += Vector3.one;
            Assert.That(hash, !Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
            m_SpriteSkin.ResetBindPose();
            Assert.That(hash, Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
            m_SpriteSkin.transform.position += Vector3.one;
            Assert.That(hash, !Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
        }

        [Test]
        public void ScaleTransform_ChangesHash()
        {
            int hash = m_SpriteSkin.CalculateTransformHash();
            m_SpriteSkin.boneTransforms[1].localScale = Vector3.one * 2f;
            Assert.That(hash, !Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
            m_SpriteSkin.ResetBindPose();
            Assert.That(hash, Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
            m_SpriteSkin.transform.localScale = Vector3.one * 2f;
            Assert.That(hash, !Is.EqualTo(m_SpriteSkin.CalculateTransformHash()));
        }

        [Test]
        public void Deform_ThrowsException_WhenBoneTransformLength_DoesNotMatch_BindPoseLength()
        {
            var outputVertices = new NativeArray<float3>(m_SpriteSkin.spriteRenderer.sprite.GetVertexCount(), Allocator.Persistent);
            var transformMatrices = new NativeArray<float4x4>(1, Allocator.Temp);

            Assert.Throws<IndexOutOfRangeException>(
                () => {
                    var sprite = m_SpriteSkin.spriteRenderer.sprite;
                    var boneWeights = sprite.GetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight);
                    var spriteVertices = sprite.GetVertexAttribute<Vector3>(UnityEngine.Rendering.VertexAttribute.Position).SliceWithStride<float3>();
                    var influences = sprite.GetVertexAttribute<BoneWeight>(UnityEngine.Rendering.VertexAttribute.BlendWeight);
                    var bindPoses = new NativeSlice<Matrix4x4>(sprite.GetBindPoses()).SliceWithStride<float4x4>();
                    
                    SpriteSkinUtility.Deform(m_SpriteSkin.transform.worldToLocalMatrix, spriteVertices, boneWeights, transformMatrices, bindPoses, outputVertices);
                },
                "BoneTransforms should have same length as BindPoses");

            outputVertices.Dispose();
            transformMatrices.Dispose();
        }

        [Test]
        public void Deform_ThrowsException_WhenBoneWeightsLength_DoesNotMatch_VerticesLength()
        {
            var outputVertices = new NativeArray<float3>(m_SpriteSkin.spriteRenderer.sprite.GetVertexCount(), Allocator.Persistent);
            var boneWeights = new NativeArray<BoneWeight>(3, Allocator.Persistent);
            var transformMatrices = new NativeArray<float4x4>(m_SpriteSkin.boneTransforms.Length, Allocator.Persistent);

            Assert.Throws<IndexOutOfRangeException>(
                () => {
                    var sprite = m_SpriteSkin.spriteRenderer.sprite;
                    var spriteVertices = sprite.GetVertexAttribute<Vector3>(UnityEngine.Rendering.VertexAttribute.Position).SliceWithStride<float3>();
                    var influences = sprite.GetVertexAttribute<BoneWeight>(UnityEngine.Rendering.VertexAttribute.BlendWeight);
                    var bindPoses = new NativeSlice<Matrix4x4>(sprite.GetBindPoses()).SliceWithStride<float4x4>();

                    SpriteSkinUtility.Deform(m_SpriteSkin.transform.worldToLocalMatrix, spriteVertices, boneWeights, transformMatrices, bindPoses, outputVertices);
                },
                "BoneWeights should have same length as input Vertices");

            outputVertices.Dispose();
            boneWeights.Dispose();
            transformMatrices.Dispose();
        }

        [Test]
        public void Deform_ThrowsException_WhenOutputVerticesLength_DoesNotMatch_VerticesLength()
        {
            var outputVertices = new NativeArray<float3>(m_SpriteSkin.spriteRenderer.sprite.GetVertexCount(), Allocator.Persistent);
            var boneWeights = new NativeArray<BoneWeight>(3, Allocator.Persistent);
            var transformMatrices = new NativeArray<float4x4>(m_SpriteSkin.boneTransforms.Length, Allocator.Persistent);

            Assert.Throws<IndexOutOfRangeException>(
                () => {
                    var sprite = m_SpriteSkin.spriteRenderer.sprite;
                    var spriteVertices = sprite.GetVertexAttribute<Vector3>(UnityEngine.Rendering.VertexAttribute.Position).SliceWithStride<float3>();
                    var influences = sprite.GetVertexAttribute<BoneWeight>(UnityEngine.Rendering.VertexAttribute.BlendWeight);
                    var bindPoses = new NativeSlice<Matrix4x4>(sprite.GetBindPoses()).SliceWithStride<float4x4>();

                    SpriteSkinUtility.Deform(m_SpriteSkin.transform.worldToLocalMatrix, spriteVertices, boneWeights, transformMatrices, bindPoses, outputVertices);
                },
                "OutputVertices should have same length as VertexCount");

            outputVertices.Dispose();
            boneWeights.Dispose();
            transformMatrices.Dispose();
        }

        [Test]
        public void SpriteSkinEntity_OnAwake_ContainsAllNeededComponentTypes()
        {
            var spriteSkinEntity = m_SpriteSkin.GetComponent<SpriteSkinEntity>();
            var entityManager = spriteSkinEntity.EntityManager;
            var entity = spriteSkinEntity.Entity;
            
            var types = entityManager.GetComponentTypes(entity).ToArray();
            var expectedTypes = new Type[]
            {
                typeof(WorldToLocal),
                typeof(SpriteComponent),
                typeof(Vertex),
                typeof(BoneTransform),
                typeof(Transform),
                typeof(SpriteRenderer),
                typeof(SpriteSkin)
            };

            foreach (var expectedType in expectedTypes)
                Assert.IsTrue(ArrayUtility.Contains(types, expectedType), "ComponentType not found: " + expectedType);
        }
    }
}
