using SFML.Graphics;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors;

namespace TankGame.Src.Extensions
{
    internal static class ListExtensions
    {
        public static void Draw(this List<IRenderable> renderables, RenderWindow renderWindow)
        {
            renderables.OrderBy(renderable => (int)renderable.RenderableRenderLayer)
                .ToList()
                .ForEach( renderable
                    => renderable.GetRenderComponents()
                        .ToList()
                        .FindAll(renderComponent => renderComponent != null)
                        .ForEach(renderComponent 
                            => renderWindow.Draw(renderComponent.Shape, new RenderStates(shader: (renderable is IShadable shadable ? shadable.CurrentShader : null)))));
        }
    }
}
