using TankGame.Actors.Brains.Thoughts;

namespace TankGame.Core; 

public interface IDto<out TDto> where TDto : Thought.Dto {
    public TDto ToDto();
}