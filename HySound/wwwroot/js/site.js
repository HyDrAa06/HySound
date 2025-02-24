﻿document.addEventListener("DOMContentLoaded", function () {
    // Get the global audio element and other elements
    const globalAudio = document.getElementById("global-audio");
    const globalAudioSource = document.getElementById("global-audio-source");
    const playButton = document.getElementById("play-btn");
    const prevButton = document.getElementById("prev-btn");
    const nextButton = document.getElementById("next-btn");
    const songImage = document.getElementById("song-image");
    const songTitle = document.getElementById("song-title");
    const artistName = document.getElementById("artist-name");
    const timebar = document.getElementById("timebar");
    const playerContainer = document.querySelector('.player-container');
    const soundBar = document.getElementById("sound-bar"); // Add sound bar

    const navLinks = document.getElementsByClassName("nav-link");

    let trackList = [];
    let currentTrackIndex = 0;
    let isPlaying = false;

    // Function to update the progress bar fill (for both timebar and sound bar)
    function updateProgressBar(bar, value) {
        bar.style.background = `linear-gradient(to right, #1db954 0%, #1db954 ${value}%, #1e1e1e ${value}%, #1e1e1e 100%)`;
    }

    // Function to load and display the stored track without auto-playing
    function loadStoredTrack() {
        const storedTrack = localStorage.getItem("currentTrack");

        if (storedTrack) {
            const { file, title, artist, image, time, isCurrentlyPlayed } = JSON.parse(storedTrack);
            globalAudioSource.src = file;
            globalAudio.load();
            globalAudio.currentTime = time || 0;
            songImage.src = image;
            songTitle.textContent = title;
            artistName.textContent = artist;
            timebar.value = (time / globalAudio.duration) * 100 || 0;
            updateProgressBar(timebar, timebar.value);

            // Set initial volume and sound bar
            globalAudio.volume = 1.0; // Default to full volume if not stored
            soundBar.value = globalAudio.volume * 100;
            updateProgressBar(soundBar, soundBar.value);

            if (isCurrentlyPlayed) {
                globalAudio.play();
                playButton.querySelector("i").classList.replace("fa-play", "fa-pause");
                isPlaying = true;
                playerContainer.classList.add('visible');
            }
        } else {
            loadTrack(0, false);
        }
    }

    for (let navLink of navLinks) {
        navLink.addEventListener("click", function () {
            saveTrackState();
        })
    }

    // Play/Pause button functionality
    playButton.addEventListener("click", function () {
        if (globalAudio.paused) {
            globalAudio.play();
            playButton.querySelector("i").classList.replace("fa-play", "fa-pause");
            isPlaying = true;
            playerContainer.classList.add('visible');
        } else {
            globalAudio.pause();
            playButton.querySelector("i").classList.replace("fa-pause", "fa-play");
            isPlaying = false;
        }
        saveTrackState();
    });

    // Previous button functionality
    prevButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex - 1 + trackList.length) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

    // Next button functionality
    nextButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex + 1) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

    // Function to load a specific track from the list
    function loadTrack(index, shouldPlay = false) {
        const track = trackList[index];
        globalAudioSource.src = track.file;
        songImage.src = track.image;
        songTitle.textContent = track.title;
        artistName.textContent = track.artist;
        globalAudio.load();
        globalAudio.currentTime = 0;
        timebar.value = 0;
        updateProgressBar(timebar, 0);

        // Reset volume to default if new track
        globalAudio.volume = 1.0;
        soundBar.value = 100;
        updateProgressBar(soundBar, 100);

        if (shouldPlay) {
            globalAudio.play();
            playButton.querySelector("i").classList.replace("fa-play", "fa-pause");
            isPlaying = true;
            playerContainer.classList.add('visible');
        }

        localStorage.setItem("currentTrack", JSON.stringify({
            file: track.file,
            title: track.title,
            artist: track.artist,
            image: track.image,
            time: globalAudio.currentTime,
            isCurrentlyPlayed: shouldPlay
        }));
    }

    // Save the current track and state to localStorage
    function saveTrackState() {
        localStorage.setItem("currentTrack", JSON.stringify({
            file: globalAudioSource.src,
            title: songTitle.textContent,
            artist: artistName.textContent,
            image: songImage.src,
            time: globalAudio.currentTime,
            isCurrentlyPlayed: isPlaying
        }));
    }

    // Update timebar as the song plays
    globalAudio.addEventListener("timeupdate", function () {
        if (!isNaN(globalAudio.duration)) {
            const progress = (globalAudio.currentTime / globalAudio.duration) * 100;
            timebar.value = progress || 0;
            updateProgressBar(timebar, timebar.value);
        }
    });

    // Set timebar to change the position of the song
    timebar.addEventListener("input", function () {
        globalAudio.currentTime = (timebar.value / 100) * globalAudio.duration;
        updateProgressBar(timebar, timebar.value);
        saveTrackState();
    });

    // Set sound bar to change volume
    soundBar.addEventListener("input", function () {
        globalAudio.volume = soundBar.value / 100;
        updateProgressBar(soundBar, soundBar.value);
    });

    // Example of tracklist initialization
    trackList = [
        {
            file: 'path-to-song1.mp3',
            title: 'Song 1',
            artist: 'Artist 1',
            image: 'path-to-image1.jpg'
        },
        {
            file: 'path-to-song2.mp3',
            title: 'Song 2',
            artist: 'Artist 2',
            image: 'path-to-image2.jpg'
        },
        {
            file: 'path-to-song3.mp3',
            title: 'Song 3',
            artist: 'Artist 3',
            image: 'path-to-image3.jpg'
        }
    ];

    // Load the last played track and respect whether it was playing or not
    loadStoredTrack();

    // --- Library Page Tabs ---
    document.getElementById('favourites-option').addEventListener('click', function () {
        toggleContent('favourites');
        updateTitle('Favourites');
        updateActiveState('favourites');
    });

    document.getElementById('playlists-option').addEventListener('click', function () {
        toggleContent('playlists');
        updateTitle('Playlists');
        updateActiveState('playlists');
    });

    document.getElementById('albums-option').addEventListener('click', function () {
        toggleContent('albums');
        updateTitle('Albums');
        updateActiveState('albums');
    });

    document.getElementById('tracklist-option').addEventListener('click', function () {
        toggleContent('tracklist');
        updateTitle('Tracklist');
        updateActiveState('tracklist');
    });

    function toggleContent(content) {
        document.getElementById('favourites-content').style.display = 'none';
        document.getElementById('playlists-content').style.display = 'none';
        document.getElementById('albums-content').style.display = 'none';
        document.getElementById('tracklist-content').style.display = 'none';

        if (content === 'favourites') {
            document.getElementById('favourites-content').style.display = 'grid';
        } else if (content === 'playlists') {
            document.getElementById('playlists-content').style.display = 'grid';
        } else if (content === 'albums') {
            document.getElementById('albums-content').style.display = 'grid';
        } else if (content === 'tracklist') {
            document.getElementById('tracklist-content').style.display = 'grid';
        }
    }

    function updateTitle(title) {
        document.getElementById('library-title').textContent = title;
    }

    function updateActiveState(option) {
        const navItems = document.querySelectorAll('.library-nav li');
        navItems.forEach(item => item.classList.remove('active'));
        document.getElementById(option + '-option').classList.add('active');
    }

    // Add event listener for play track buttons in your library
    document.addEventListener("click", function (e) {
        const btn = e.target.closest('.play-track-btn');
        if (!btn) return;

        const file = btn.dataset.src;
        const title = btn.dataset.title;
        const artist = btn.dataset.artist;
        const image = btn.dataset.image;

        trackList = [{ file, title, artist, image }];
        loadTrack(0, true);
    });
});