﻿using OKHOSTING.UI.Controls.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.UI.Controls.Forms
{
	public class Form
	{
		#region Fields and properties

		/// <summary>
		/// The grid that will be populated with all the fields. 
		/// Add this content to your page after calling DataBind()
		/// </summary>
		public IGrid Content
		{
			get;
			protected set;
		}

		/// <summary>
		/// Collection of fields that will be displayed in the current form
		/// </summary>
		public readonly List<FormField> Fields = new List<FormField>();

		/// <summary>
		/// Determines wether the labels will be shown at the right or the top of each value
		/// </summary>
		public CaptionPosition LabelPosition { get; set; } = CaptionPosition.Left;

		/// <summary>
		/// Gets or sets the number of columns to display horizontally
		/// </summary>
		public int RepeatColumns { get; set; } = 1;

		public bool IsValid
		{
			get
			{
				return Fields.Where(f => f.IsValid).Count() == 0;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Creates all cells and controls based on the Fields collection
		/// </summary>
		public void DataBind()
		{
			//the grid that will actually be displayed to the user and contain all the fields
			Content = Platform.Current.Create<IGrid>();

			//column counter
			int currentColumn = 0;

			//if there are more than one category, group fields by category
			bool categorize = false;
			string currentCategory = string.Empty;
			int categoryCount = 0;

			#region Sort fields and init values

			//sort fields by required, category
			Fields.Sort
			(
				delegate (FormField f1, FormField f2)
				{
					//set categories to empty in case they are null
					if (string.IsNullOrWhiteSpace(f1.Category)) f1.Category = "General";
					if (string.IsNullOrWhiteSpace(f2.Category)) f2.Category = "General";

					//set a "Category.SortOrder.Name" string comparission
					string val1, val2;

					val1 = f1.Category + "." + (f1.ValueControl.Enabled ? "1" : "0") + "." + (f1.Required ? "0" : "1") + (f1.TableWide ? "1" : "0") + f1.SortOrder.ToString("0:000000000000000") + f1.Name;
					val2 = f2.Category + "." + (f2.ValueControl.Enabled ? "1" : "0") + "." + (f2.Required ? "0" : "1") + (f2.TableWide ? "1" : "0") + f2.SortOrder.ToString("0:000000000000000") + f2.Name;

					//compare categories
					return val1.CompareTo(val2);
				}
			);

			//init all fields
			foreach (FormField field in Fields)
			{
				field.Container = this;

				if (currentCategory != field.Category)
				{
					currentCategory = field.Category;
					categoryCount++;
				}
			}

			//decide wether or not to categorize
			if (categoryCount > 1) categorize = true;

			#endregion

			#region Create cells with LabelPosition = Left

			//If label position is left
			if (this.LabelPosition == CaptionPosition.Left)
			{
				Content.ColumnCount = RepeatColumns < 2 ? 2 : (int) Math.Max(RepeatColumns, Math.Ceiling((decimal) RepeatColumns / 2) * 2);

				//add every field
				foreach (FormField field in Fields)
				{
					//if a new category is found, add a new category row
					if (currentCategory != field.Category && categorize)
					{
						//create new category row
						CreateCategoryRow(field.Category, Content);

						currentColumn = 0;
						currentCategory = field.Category;
					}

					//if a field with TableWide = true is found, create a row for this field only
					if (field.TableWide)
					{
						//create new rows for this field
						CreateTableWideRow(field, Content);
						
						//reset counter
						currentColumn = 0;

						//go to next field
						continue;
					}

					//create new row if this is the first row or if RepeatColumns has been reached 
					if (Content.RowCount == 0 || currentColumn >= RepeatColumns)
					{
						Content.RowCount++;
						currentColumn = 0;
					}

					//Add name cell
					Content.SetContent(Content.RowCount - 1, currentColumn, field.CaptionControl);
					currentColumn++;

					//Add value cell
					Content.SetContent(Content.RowCount - 1, currentColumn, field.ValueControl);
					currentColumn++;
				}
			}

			#endregion

			#region Create cells with LabelPosition = Top

			//If label position is top
			else
			{
				Content.ColumnCount = RepeatColumns;

				//add every field
				foreach (FormField field in Fields)
				{
					//if a new category is found, add a new category row
					if (currentCategory != field.Category && categorize)
					{
						//create new category row
						CreateCategoryRow(field.Category, Content);

						//reset counter
						currentColumn = 0;

						//update current category
						currentCategory = field.Category;
					}

					//if a field with TableWide = true is found, create a row for this field only
					if (field.TableWide)
					{
						//create new rows for this field
						CreateTableWideRow(field, Content);

						//reset counter
						currentColumn = 0;

						//go to next field
						continue;
					}

					//Add name cells to the almost last row
					Content.SetContent(Content.RowCount - 2, currentColumn, field.CaptionControl);

					//Add value cell to the last row
					Content.SetContent(Content.RowCount - 1, currentColumn, field.ValueControl);

					//increment column counter
					currentColumn++;

					//create new column if RepeatColumns has been reached
					if (currentColumn >= RepeatColumns)
					{
						currentColumn = 0;

						//add one row for captions and another for values
						Content.RowCount++;
						Content.RowCount++;
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// Searches for a field with the specified Id
		/// </summary>
		/// <param name="fieldName">Id of the field to be searched</param>
		/// <returns>If found, the field with the specified Id, null otherwise</returns>
		public FormField this[string fieldName]
		{
			get
			{
				foreach (FormField f in Fields)
				{
					if (f.Name == fieldName) return f;
				}

				return null;
			}
		}

		/// <summary>
		/// Returns the value entered by the user for a specified field
		/// </summary>
		/// <param name="fieldId">Id of the field which value is to be retrieved</param>
		/// <returns>Value entered by the user in the specified field</returns>
		public object GetValue(string fieldName)
		{
			//search fields collection by id
			foreach (FormField f in Fields)
			{
				if (f.Name == fieldName) return f.Value;
			}

			//if no matching fielod was found, return null
			return null;
		}

		//protected

		/// <summary>
		/// Creates a division row with a category name
		/// </summary>
		/// <param name="category">Name of the category</param>
		protected void CreateCategoryRow(string category, IGrid grid)
		{
			ILabel caption = Platform.Current.Create<ILabel>();
			caption.Text = category;

			grid.RowCount++;
			grid.SetContent(grid.RowCount - 1, 0, caption);
			grid.SetColumnSpan(grid.ColumnCount, caption);
		}

		/// <summary>
		/// Creates a row that includes a label and another row for the value of a TableWide field
		/// </summary>
		/// <param name="field">FormField that will be included in these rows</param>
		protected void CreateTableWideRow(FormField field, IGrid grid)
		{
			//add a full row for the caption
			grid.RowCount++;
			grid.SetContent(grid.RowCount - 1, 0, field.CaptionControl);
			grid.SetColumnSpan(grid.ColumnCount, field.CaptionControl);

			//and another full row for the value
			grid.RowCount++;
			grid.SetContent(grid.RowCount - 1, 0, field.ValueControl);
			grid.SetColumnSpan(grid.ColumnCount, field.ValueControl);
		}

		#endregion
	}
}