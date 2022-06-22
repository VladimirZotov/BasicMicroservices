using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _dbContext;
        public CommandRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateCommand(int platformId, Command command)
        {

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (!PlatformExists(platformId))
            {
                throw new ArgumentNullException("Platform doesn't exist");
            }

            command.PlatformId = platformId;

            _dbContext.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _dbContext.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _dbContext.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _dbContext.Commands.FirstOrDefault(x => x.PlatformId == platformId && x.Id == commandId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _dbContext.Commands.Where(x => x.PlatformId == platformId).ToList();
        }

        public bool PlatformExists(int platformId)
        {
            return _dbContext.Platforms.Any(x => x.Id == platformId);
        }

        public bool SaveChanges()
        {
            return _dbContext.SaveChanges() >= 0;
        }
    }
}
