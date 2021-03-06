﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QuadTree
{
    
    /// <summary>
    /// Interface to define Rect, so that QuadTree knows how to store the object.
    /// </summary>
    public interface IHasRectangle
    {
        /// <summary>
        /// The rectangle that defines the object's boundaries.
        /// </summary>
        Rect Rect { get; }
    }

    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object iheriting from IHasRect.</typeparam>
    public class QuadTree<T> where T : IHasRectangle
    {
        #region Constants
        // How many objects can exist in a QuadTree before it sub divides itself
        private const int MAX_OBJECTS_PER_NODE = 2;
        #endregion

        #region Private Members
        private List<T> m_objects = null;       // The objects in this QuadTree
        private Rect m_rect;               // The area this QuadTree represents

        private QuadTree<T> m_childTL = null;   // Top Left Child
        private QuadTree<T> m_childTR = null;   // Top Right Child
        private QuadTree<T> m_childBL = null;   // Bottom Left Child
        private QuadTree<T> m_childBR = null;   // Bottom Right Child
        #endregion

        #region Public Properties
        /// <summary>
        /// The area this QuadTree represents.
        /// </summary>
        public Rect QuadRect { get { return m_rect; } }

        /// <summary>
        /// The top left child for this QuadTree
        /// </summary>
        public QuadTree<T> TopLeftChild { get { return m_childTL; } }

        /// <summary>
        /// The top right child for this QuadTree
        /// </summary>
        public QuadTree<T> TopRightChild { get { return m_childTR; } }

        /// <summary>
        /// The bottom left child for this QuadTree
        /// </summary>
        public QuadTree<T> BottomLeftChild { get { return m_childBL; } }

        /// <summary>
        /// The bottom right child for this QuadTree
        /// </summary>
        public QuadTree<T> BottomRightChild { get { return m_childBR; } }

        /// <summary>
        /// The objects contained in this QuadTree at it's level (ie, excludes children)
        /// </summary>
        public List<T> Objects { get { return m_objects; } }

        /// <summary>
        /// How many total objects are contained within this QuadTree (ie, includes children)
        /// </summary>
        public int Count { get { return this.ObjectCount(); } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        public QuadTree(Rect rect)
        {
            m_rect = rect;
        }

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area reactangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        public QuadTree(int x, int y, int width, int height)
        {
            m_rect = new Rect(x, y, width, height);
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Add an item to the object list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(T item)
        {
            if (m_objects == null)
                m_objects = new List<T>();

            m_objects.Add(item);
        }

        /// <summary>
        /// Remove an item from the object list.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        private void Remove(T item)
        {
            if (m_objects != null && m_objects.Contains(item))
                m_objects.Remove(item);
        }

        /// <summary>
        /// Get the total for all objects in this QuadTree, including children.
        /// </summary>
        /// <returns>The number of objects contained within this QuadTree and its children.</returns>
        private int ObjectCount()
        {
            int count = 0;

            // Add the objects at this level
            if (m_objects != null) count += m_objects.Count;

            // Add the objects that are contained in the children
            if (m_childTL != null)
            {
                count += m_childTL.ObjectCount();
                count += m_childTR.ObjectCount();
                count += m_childBL.ObjectCount();
                count += m_childBR.ObjectCount();
            }

            return count;
        }

        /// <summary>
        /// Subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        private void Subdivide()
        {
            // We've reached capacity, subdivide...
            Vector2 size = new Vector2(m_rect.width / 2, m_rect.height / 2);
            Vector2 mid = new Vector2(m_rect.x + size.x, m_rect.y + size.y);

            m_childTL = new QuadTree<T>(new Rect(m_rect.xMin, m_rect.yMin, size.x, size.y));
            m_childTR = new QuadTree<T>(new Rect(mid.x, m_rect.yMin, size.x, size.y));
            m_childBL = new QuadTree<T>(new Rect(m_rect.xMin, mid.y, size.x, size.y));
            m_childBR = new QuadTree<T>(new Rect(mid.x, mid.y, size.x, size.y));

            // If they're completely contained by the quad, bump objects down
            for (int i = 0; i < m_objects.Count; i++)
            {
                QuadTree<T> destTree = GetDestinationTree(m_objects[i]);

                if (destTree != this)
                {
                    // Insert to the appropriate tree, remove the object, and back up one in the loop
                    destTree.Insert(m_objects[i]);
                    Remove(m_objects[i]);
                    i--;
                }
            }
        }

        /// <summary>
        /// Get the child Quad that would contain an object.
        /// </summary>
        /// <param name="item">The object to get a child for.</param>
        /// <returns></returns>
        private QuadTree<T> GetDestinationTree(T item)
        {
            // If a child can't contain an object, it will live in this Quad
            QuadTree<T> destTree = this;

            if (Utility.Contains(m_childTL.QuadRect, item.Rect))
            {
                destTree = m_childTL;
            }
            else if (Utility.Contains(m_childTR.QuadRect, item.Rect))
            {
                destTree = m_childTR;
            }
            else if (Utility.Contains(m_childBL.QuadRect, item.Rect))
            {
                destTree = m_childBL;
            }
            else if (Utility.Contains(m_childBR.QuadRect, item.Rect))
            {
                destTree = m_childBR;
            }

            return destTree;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Clears the QuadTree of all objects, including any objects living in its children.
        /// </summary>
        public void Clear()
        {
            // Clear out the children, if we have any
            if (m_childTL != null)
            {
                m_childTL.Clear();
                m_childTR.Clear();
                m_childBL.Clear();
                m_childBR.Clear();
            }

            // Clear any objects at this level
            if (m_objects != null)
            {
                m_objects.Clear();
                m_objects = null;
            }

            // Set the children to null
            m_childTL = null;
            m_childTR = null;
            m_childBL = null;
            m_childBR = null;
        }

        /// <summary>
        /// Deletes an item from this QuadTree. If the object is removed causes this Quad to have no objects in its children, it's children will be removed as well.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Delete(T item)
        {
            // If this level contains the object, remove it
            bool objectRemoved = false;
            if (m_objects != null && m_objects.Contains(item))
            {
                Remove(item);
                objectRemoved = true;
            }

            // If we didn't find the object in this tree, try to delete from its children
            if (m_childTL != null && !objectRemoved)
            {
                m_childTL.Delete(item);
                m_childTR.Delete(item);
                m_childBL.Delete(item);
                m_childBR.Delete(item);
            }

            if (m_childTL != null)
            {
                // If all the children are empty, delete all the children
                if (m_childTL.Count == 0 &&
                    m_childTR.Count == 0 &&
                    m_childBL.Count == 0 &&
                    m_childBR.Count == 0)
                {
                    m_childTL = null;
                    m_childTR = null;
                    m_childBL = null;
                    m_childBR = null;
                }
            }
        }

        /// <summary>
        /// Insert an item into this QuadTree object.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        public void Insert(T item)
        {
            // If this quad doesn't intersect the items rectangle, do nothing
            if (!Utility.Intersects(m_rect, item.Rect))
                return;

            if (m_objects == null ||
                (m_childTL == null && m_objects.Count + 1 <= MAX_OBJECTS_PER_NODE))
            {
                // If there's room to add the object, just add it
                Add(item);
            }
            else
            {
                // No quads, create them and bump objects down where appropriate
                if (m_childTL == null)
                {
                    Subdivide();
                }

                // Find out which tree this object should go in and add it there
                QuadTree<T> destTree = GetDestinationTree(item);
                if (destTree == this)
                {
                    Add(item);
                }
                else
                {
                    destTree.Insert(item);
                }
            }
        }

        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        public void GetObjects(Rect rect, ref List<T> results)
        {
            // We can't do anything if the results list doesn't exist
            if (results != null)
            {
                if (Utility.Contains(rect, m_rect))
                {
                    // If the search area completely contains this quad, just get every object this quad and all it's children have
                    GetAllObjects(ref results);
                }
                else if (Utility.Intersects(rect, m_rect))
                {
                    // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                    if (m_objects != null)
                    {
                        for (int i = 0; i < m_objects.Count; i++)
                        {
                            if (Utility.Intersects(rect, m_objects[i].Rect))
                            {
                                results.Add(m_objects[i]);
                            }
                        }
                    }

                    // Get the objects for the search rectangle from the children
                    if (m_childTL != null)
                    {
                        m_childTL.GetObjects(rect, ref results);
                        m_childTR.GetObjects(rect, ref results);
                        m_childBL.GetObjects(rect, ref results);
                        m_childBR.GetObjects(rect, ref results);
                    }
                }
            }
        }

        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        /// <param name="results">A reference to a list in which to store the objects.</param>
        public void GetAllObjects(ref List<T> results)
        {
            // If this Quad has objects, add them
            if (m_objects != null)
                results.AddRange(m_objects);

            // If we have children, get their objects too
            if (m_childTL != null)
            {
                m_childTL.GetAllObjects(ref results);
                m_childTR.GetAllObjects(ref results);
                m_childBL.GetAllObjects(ref results);
                m_childBR.GetAllObjects(ref results);
            }
        }
        #endregion
    }
}
