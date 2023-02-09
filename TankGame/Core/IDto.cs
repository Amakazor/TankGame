using TankGame.Actors.Brains.Thoughts;

namespace TankGame.Core; 

public interface IDto<TDto> where TDto : Thought.Dto {
    public TDto ToDto();
}