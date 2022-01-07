using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JUtils
{
    public static class JUtilsClass
    {

        public static Vector2 RandomPointInCollider (Collider2D collider)
        {
            Vector3 minBound = collider.bounds.min;
            Vector3 maxBound = collider.bounds.max;

            Vector3 randomPoint;

            do
            {
                randomPoint =
                  new Vector3(
                    Random.Range(minBound.x, maxBound.x),
                    Random.Range(minBound.y, maxBound.y),
                    Random.Range(minBound.z, maxBound.z)

                  );
            } while (!collider.OverlapPoint(randomPoint));

            return randomPoint;
        }

        //assuming each value in the array represents an weighting, will randomly pick one of the index's to return based on those weightings
        public static int getIndexByWeighting(int[] probabilities)
        {
            int totalWeighting = 0;
            foreach (int val in probabilities)
                totalWeighting += val;

            int roll = Random.Range(0, totalWeighting);

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (roll < probabilities[i])
                    return i;
                else
                    roll = roll - probabilities[i];
            }

            return probabilities[probabilities.Length - 1];
        }


        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 0)
        {
            if (color == null) color = Color.black;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }

        // Create Text in the World
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
        public static Vector2 getCentreOfTranforms (List<Transform> list)
        {
            float x = 0f;
            float y = 0f;
            foreach (Transform t in list)
            {
                x += t.position.x;
                y += t.position.y;
            }

            x = x / list.Count;
            y = y / list.Count;

            return new Vector2(x, y);
        }

        public static Vector2 getCentreOfMinions (List<MinionController> list)
        {
            return getCentreOfTranforms(getListOfTransforms(list));
        }

        public static Transform getNearestTransformToPointFromList (Vector2 origin, List<Transform> list)
        {
            float closestDistance = Mathf.Infinity;
            Transform closest = null;
            foreach (Transform t in list)
            {
                if (Vector2.Distance(origin, t.position) < closestDistance)
                {
                    closestDistance = Vector2.Distance(origin, t.position);
                    closest = t;
                }
            }

            return closest;
        }

        public static MinionController getNearestMinionToPointFromList (MinionController origin, List<MinionController> list)
        {
            return getNearestTransformToPointFromList(origin.transform.position, getListOfTransforms(list)).GetComponent<MinionController>();
        }

        public static MinionController getNearestMinionToPointFromList (Vector2 origin, List<MinionController> list)
        {
            return getNearestTransformToPointFromList(origin, getListOfTransforms(list)).GetComponent<MinionController>();
        }

        public static List<Transform> getListOfTransforms (List<MinionController> list)
        {
            List<Transform> tList = new List<Transform>();

            foreach (MinionController m in list)
                tList.Add(m.transform);

            return tList;
        }

        public static Vector2 getRandomPointInBounds(Bounds b)
        {
            return new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
        }

        //made in JUtils in case I find a better way of doing it (e.g. using partitions)
        public static Collider2D[] getAllCollidersInRadius(Vector2 origin, float radius, string tag = "null")
        {
            Collider2D[] output; 
            output = Physics2D.OverlapCircleAll(origin, radius);
            return output;
        }

        public static float convertBoundsToRadius(Bounds b, float additionalRadius)
        {
            float maxExtent = Mathf.Max(b.extents.x, b.extents.y);
            return maxExtent + additionalRadius;
        }

        public static Vector2 getDirection (Vector2 origin, Vector2 target)
        {
            return (target - origin).normalized;
        }

        public static Vector2 getDirection (Transform origin, Transform target)
        {
            return (target.position - origin.position).normalized;
        }
    }
}
