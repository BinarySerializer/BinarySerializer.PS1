using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.PS1
{
    public class PS1_VRAM
    {
		private const int PageHeight = 256;
		private const int PageWidth = 128; // for 8-bit CLUT.

		protected List<ReservedBlock> ReservedBlocks { get; } = new List<ReservedBlock>();
		public Page[][] Pages { get; } = new Page[2][]; // y, x
        public List<Palette> Palettes { get; } = new List<Palette>(); // Optional collection of any palettes in the V-RAM
        public int CurrentXPage { get; set; }
		public int CurrentYPage { get; set; }
		public int NextYInPage { get; set; }

		public void ReserveBlock(int x, int y, int width, int height) => ReservedBlocks.Add(new ReservedBlock()
        {
            x = x, 
            y = y, 
            width = width, 
            height = height
        });
        private bool AnyBlockReserved(int x, int y) => ReservedBlocks.Any(b => (x >= b.x && x < b.x + b.width) && (y >= b.y && y < b.y + b.height));

        public void AddData(byte[] data, int width) 
        {
			if (data == null) 
                return;

			/*if (pages[currentYPage] == null || pages[currentYPage].Length == 0) {
				pages[currentYPage] = new Page[currentXPage + (width / page_width)];
			}
			if (pages[currentYPage].Length < currentXPage + (width / page_width)) {
				Array.Resize(ref pages[currentYPage], currentXPage + (width / page_width));
			}*/

			int height = data.Length / width + ((data.Length % width != 0) ? 1 : 0);

            int yInPageMod = 0;
            
            if (height <= 0 || width <= 0)
                return;
            
            int yInPage = 0;
            var curPageY = CurrentYPage;
            var curStartPageX = CurrentXPage;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++) 
                {
                    curStartPageX = CurrentXPage;
                    var curPageX = curStartPageX + (x / PageWidth);
                    curPageY = CurrentYPage + ((NextYInPage + yInPageMod + y) / PageHeight);
                    var xInPage = x % PageWidth;
                    yInPage = (NextYInPage + yInPageMod + y) % PageHeight;

                    while (curPageY > 1) 
                    {
                        // Wrap
                        curPageY -= 2;
                        curPageX += (width / PageWidth);
                        curStartPageX += (width / PageWidth);
                    }

                    int totalX = curPageX * PageWidth + xInPage;
                    int totalY = curPageY * PageHeight + yInPage;

                    while (AnyBlockReserved(totalX, totalY)) 
                    {
                        // Wrap
                        curPageX += (width / PageWidth);
                        curStartPageX += (width / PageWidth);

                        yInPageMod += (PageHeight - yInPage) + (curPageY == 0 ? PageHeight : 0);
                        curPageY = 0;
                        yInPage = (NextYInPage + yInPageMod + y) % PageHeight;

                        totalX = curPageX * PageWidth + xInPage;
                        totalY = curPageY * PageHeight + yInPage;
                    }

                    if (Pages[curPageY] == null || Pages[curPageY].Length == 0)
                        Pages[curPageY] = new Page[curPageX+1];

                    if (Pages[curPageY].Length < curPageX + 1)
                        Array.Resize(ref Pages[curPageY], curPageX + 1);

                    Pages[curPageY][curPageX] ??= new Page();
                    Pages[curPageY][curPageX].SetByte(xInPage, yInPage, data[y * width + x]);
                }
            }

            CurrentXPage = curStartPageX;
            CurrentYPage = curPageY;

            NextYInPage = yInPage + 1;

            if (NextYInPage >= PageHeight) 
            {
                // Change page
                NextYInPage -= PageHeight;
                CurrentYPage++;

                if (CurrentYPage > 1) 
                {
                    //  Wrap
                    CurrentXPage += (width / PageWidth);
                    CurrentYPage -= 2;
                }
            }

			/*if (data != null) {
				if (pages.Count == 0) {
					pages.Add(new Page(pageWidth));
				}
				byte[] newData = pages.Last().AddData(data);
				while (newData != null) {
					pages.Add(new Page(pageWidth));
					newData = pages.Last().AddData(data);
				}
			}*/
		}

		public void AddDataAt(int startXPage, int startYPage, int startX, int startY, byte[] data, int width) 
        {
			if (data == null)
                return;

			int height = (startX + data.Length) / width + (((startX + data.Length) % width != 0) ? 1 : 0);

            if (height <= 0 || width <= 0) 
                return;

            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < height; y++) 
                {
                    var curPageX = startXPage + ((startX + x) / PageWidth);
                    var curPageY = startYPage + ((startY + y) / PageHeight);
                    var xInPage = (startX + x) % PageWidth;
                    var yInPage = (startY + y) % PageHeight;

                    while (curPageY > 1) 
                    {
                        // Wrap
                        curPageY -= 2;
                        curPageX += (width / PageWidth);
                    }

                    if (Pages[curPageY] == null || Pages[curPageY].Length == 0) 
                        Pages[curPageY] = new Page[curPageX+1];

                    if (Pages[curPageY].Length < curPageX + 1)
                        Array.Resize(ref Pages[curPageY], curPageX + 1);

                    Pages[curPageY][curPageX] ??= new Page();
                    Pages[curPageY][curPageX].SetByte(xInPage, yInPage, data[y * width + x]);
                }
            }
        }

        public void AddPalette(RGBA5551Color[] colors, int pageX, int pageY, int x, int y)
        {
            AddDataAt(pageX, pageY, x, y, colors.SelectMany(c => BitConverter.GetBytes((ushort)c.ColorValue)).ToArray(), 512);
            Palettes.Add(new Palette(colors, pageX * PageWidth + x, pageY * PageHeight + y));
        }

		private Page GetPage(int x, int y) 
        {
			try 
            {
				if (x >= Pages[y].Length) 
                    return null;

				return Pages[y][x];
			}
            catch (Exception ex)
            {
                throw new Exception($"Error getting VRAM page at ({x}, {y})", ex);
			}
		}

		public byte GetPixel8(int pageX, int pageY, int x, int y) 
        {
            while (x >= PageWidth) 
            {
				pageX++;
				x -= PageWidth;
			}
			
            if (y > PageHeight) 
            {
				pageY++;
				y -= PageHeight;
			}

			Page page = GetPage(pageX, pageY);

            if (page == null) 
                return 0;
			
            return page.GetByte(x, y);
		}

		public RGBA5551Color GetColor1555(int pageX, int pageY, int x, int y) 
        {
			byte b0 = GetPixel8(pageX, pageY, x * 2, y);
			byte b1 = GetPixel8(pageX, pageY, (x * 2) + 1, y);
			ushort col = (ushort)(b0 | (b1 << 8));
			return new RGBA5551Color(col);
		}

		public class Page 
        {
			public byte[] Data { get; } = new byte[PageWidth * PageHeight];
			
            public byte GetByte(int x, int y) => Data[y * PageWidth + x];
            public void SetByte(int x, int y, byte value) => Data[y * PageWidth + x] = value;
            
            /*public int width;
			public byte[] data;

			public Page(int width) {
				this.width = width;
			}

			public byte GetByte(int x, int y) {
				return data[y * width + x];
			}

			// We assume that page data is 8bit colors
			public byte[] AddData(byte[] data) {
				int curDataCount = data?.Length ?? 0;
				int newDataCount = Math.Min(curDataCount + data.Length, width * 256);
				int dataToCopy = newDataCount - curDataCount;
				if (dataToCopy > 0) {
					if (this.data == null) {
						this.data = new byte[newDataCount];
					} else {
						Array.Resize(ref this.data, newDataCount);
					}
					Array.Copy(data, 0, this.data, curDataCount, dataToCopy);
					if (data.Length > dataToCopy) {
						byte[] newData = new byte[data.Length - dataToCopy];
						Array.Copy(data, dataToCopy, newData, 0, newData.Length);
						return newData;
					} else {
						return null;
					}
				}
				return data;
			}*/
		}

        public class Palette
        {
            public Palette(RGBA5551Color[] colors, int x, int y)
            {
                Colors = colors;
                X = x;
                Y = y;
            }

            public RGBA5551Color[] Colors { get; }
            public int X { get; }
            public int Y { get; }
        }

		public struct ReservedBlock
        {
			public int x;
			public int y;
			public int width;
			public int height;
		}
	}
}