﻿namespace OKHOSTING.UI.Controls.Layout
{
	/// <summary>
	/// It is a control that represents a grid where we can store objects.
	/// <para xml:lang="es">
	/// Es un control que representa una cuadricula donde podemos almacenar objetos.
	/// </para>
	/// </summary>
	public interface IGrid : IControl
	{
		int ColumnCount { get; set; }
		int RowCount { get; set; }

		IControl GetContent(int row, int column);
		void SetContent(int row, int column, IControl content);

		void SetColumnSpan(int columnSpan, IControl content);
		int GetColumnSpan(IControl content);

		void SetRowSpan(int rowSpan, IControl content);
		int GetRowSpan(IControl content);

		void SetWidth(int column, double width);
		double GetWidth(int column);

		void SetHeight(int row, double height);
		double GetHeight(int row);

		/// <summary>
		/// Space that this grid will set between one cell and another
		/// <para xml:lang="es">
		/// Es el espacio que el grid fijara entre una celda y otra.
		/// </para>
		/// </summary>
		Thickness CellMargin { get; set; }

		/// <summary>
		/// Space that this grid will set between a cell's border and that cell's content
		/// <para xml:lang="es">
		/// Es el espacio que el grid fijara entre el limite de la celda y el contenido de la celda.
		/// </para>
		/// </summary>
		Thickness CellPadding { get; set; }
	}

	/// <summary>
	/// Returns all controls inside the grid.
	/// <para xml:lang="es">
	/// Devuelve todos los controles que hay dentro del grid.
	/// </para>
	/// </summary>
	public static class IGridExtensions
	{
		/// <summary>
		/// Gets all controlls.
		/// <para xml:lang="es">
		/// Obtenemos todos los controles que contiene el grid.
		/// </para>
		/// </summary>
		/// <returns>The all controlls.
		/// <para xml:lang="es">Todos los controles.</para>
		/// </returns>
		/// <param name="grid">Grid.</param>
		public static System.Collections.Generic.IEnumerable<IControl> GetAllControlls(this IGrid grid)
		{
			for (int row = 0; row < ((IGrid) grid).RowCount; row++)
			{
				for (int column = 0; column < ((IGrid) grid).ColumnCount; column++)
				{
					yield return ((IGrid) grid).GetContent(row, column);
				}
			}
		}
	}
}