namespace TerminalRaid.Services;

public static class PlayerSessionService
{
    public static Task<string> InitializeSessionAsync(string username)
    {
        var bootSequence = """
            * Loading custom binary format handlers ...                             [ ok ]
            * Checking local filesystems ...                                       [ ok ]
            * Remounting filesystems ...                                           [ ok ]
            * Updating /etc/mtab ...                                               [ ok ]
            * Activating swap devices ...                                          [ ok ]
            * Mounting local filesystems ...                                       [ ok ]
            * Configuring kernel parameters ...                                    [ ok ]
            * Creating user login records ...                                      [ ok ]
            * Starting dbus ...                                                    [ ok ]
            * Starting elogind ...                                                 [ ok ]
            * Setting hostname to raidbox from /etc/hostname ...                   [ ok ]
            * Setting terminal encoding [UTF-8] ...                                [ ok ]
            * Setting keyboard mode [UTF-8] ...                                    [ ok ]
            * Loading key mappings [us] ...                                        [ ok ]
            * Bringing up network interface lo ...
                Caching network module dependencies
                127.0.0.1/8 ...
                Adding routes
                127.0.0.0/8 via 127.0.0.1 ...                                       [ ok ]
            * Saving random number generator seed ...                              [ ok ]
            * Seeding 256 bits and crediting ...                                   [ ok ]
            * Starting default runlevel
            * Starting NetworkManager ...
            Connecting...............
            * NetworkManager is active — connection established via eth0           [ ok ]
            * Starting raid-local ...                                              [ ok ]
            * Mounting RAID arrays: /dev/md0 (/data)                               [ ok ]
            * Starting SSH daemon ...                                              [ ok ]
            * Starting local ...                                                   [ ok ]
            
            RaidOS GNU/Linux 1.0 (tty1)
            Kernel 6.6.9-zen-raid (x86_64)
            
            Hint: Num Lock on
            IP: 192.168.1.42  ::  Gateway: 192.168.1.1  ::  Device: eth0
            
            """;

        var result = $"Welcome, {username}\n{bootSequence}{username}@{username}-PC:$~";
        return Task.FromResult(result);
    }
}