// site.js
document.addEventListener("DOMContentLoaded", function () {
    // Check if we're on the AlbumDetails or PlaylistDetails page
    const isAlbumDetailsPage = window.location.pathname.includes('AlbumDetails');
    const isPlaylistDetailsPage = window.location.pathname.includes('PlaylistDetails');
    if (isAlbumDetailsPage || isPlaylistDetailsPage) return;

   
    // Get references to DOM elements
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
    const soundBar = document.getElementById("sound-bar");

    const navLinks = document.getElementsByClassName("nav-link");

    // State variables
    let trackList = [];
    let currentTrackIndex = 0;
    let isPlaying = false;
    let selectedAlbumId = null;
    let currentTracks = [];
    let selectedPlaylistId = null;

    // Helper function to update progress bars
    function updateProgressBar(bar, value) {
        bar.style.background = `linear-gradient(to right, #1db954 0%, #1db954 ${value}%, #1e1e1e ${value}%, #1e1e1e 100%)`;
    }

    // Load stored track state from localStorage
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
            globalAudio.volume = 1.0;
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

    // Save track state on navigation
    for (let navLink of navLinks) {
        navLink.addEventListener("click", saveTrackState);
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

    // Previous track button
    prevButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex - 1 + trackList.length) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

    // Next track button
    nextButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex + 1) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

    // Load a track into the player
    function loadTrack(index, shouldPlay = false) {
        const track = trackList[index];
        console.log('Loading track:', track);

        globalAudioSource.src = track.file;
        songImage.src = track.image || 'path/to/default/image.jpg';
        console.log('Image set to:', songImage.src);

        songTitle.textContent = track.title || 'Unknown Title';
        console.log('Title set to:', songTitle.textContent);

        artistName.textContent = track.artist || 'Unknown Artist';
        console.log('Artist set to:', artistName.textContent);

        globalAudio.load();
        globalAudio.currentTime = 0;
        timebar.value = 0;
        updateProgressBar(timebar, 0);
        globalAudio.volume = 1.0;
        soundBar.value = 100;
        updateProgressBar(soundBar, 100);

        if (shouldPlay) {
            globalAudio.addEventListener('canplay', function playTrack() {
                globalAudio.play();
                playButton.querySelector("i").classList.replace("fa-play", "fa-pause");
                isPlaying = true;
                playerContainer.classList.add('visible');
                globalAudio.removeEventListener('canplay', playTrack);
            });
        } else {
            playButton.querySelector("i").classList.replace("fa-pause", "fa-play");
            isPlaying = false;
            playerContainer.classList.remove('visible');
        }
        saveTrackState();
    }

    // Save current track state to localStorage
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

    // Update timebar as audio plays
    globalAudio.addEventListener("timeupdate", function () {
        if (!isNaN(globalAudio.duration)) {
            const progress = (globalAudio.currentTime / globalAudio.duration) * 100;
            timebar.value = progress || 0;
            updateProgressBar(timebar, timebar.value);
        }
    });

    // Seek audio when timebar is adjusted
    timebar.addEventListener("input", function () {
        globalAudio.currentTime = (timebar.value / 100) * globalAudio.duration;
        updateProgressBar(timebar, timebar.value);
        saveTrackState();
    });

    // Adjust volume with sound bar
    soundBar.addEventListener("input", function () {
        globalAudio.volume = soundBar.value / 100;
        updateProgressBar(soundBar, soundBar.value);
    });

    // Default track list (for testing or initial load)
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

    loadStoredTrack();

    // Library navigation event listeners
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

    

    // Toggle library content visibility
    function toggleContent(content) {
        document.getElementById('favourites-content').style.display = 'none';
        document.getElementById('playlists-content').style.display = 'none';
        document.getElementById('albums-content').style.display = 'none';
        document.getElementById('selected-album-tracks').style.display = 'none';
        document.getElementById('selected-playlist-tracks').style.display = 'none';

        if (content === 'favourites') {
            document.getElementById('favourites-content').style.display = 'grid';
        } else if (content === 'playlists') {
            if (selectedPlaylistId) {
                showPlaylistTracks(selectedPlaylistId);
            } else {
                document.getElementById('playlists-content').style.display = 'grid';
            }
        } else if (content === 'albums') {
            if (selectedAlbumId) {
                showAlbumTracks(selectedAlbumId);
            } else {
                document.getElementById('albums-content').style.display = 'grid';
            }
        
        }
    }

    // Update library title
    function updateTitle(title) {
        document.getElementById('library-title').textContent = title;
    }

    // Update active navigation item
    function updateActiveState(option) {
        const navItems = document.querySelectorAll('.library-nav li');
        navItems.forEach(item => item.classList.remove('active'));
        document.getElementById(option + '-option').classList.add('active');
    }

    // Main click event listener for albums, playlists, and tracks
    document.addEventListener("click", function (e) {
        // Handle album card clicks
        const albumCard = e.target.closest('.album-card');
        if (albumCard && albumCard.dataset.type === 'album') {
            const playButton = albumCard.querySelector('.album-play-btn');
            if (e.target === playButton || playButton.contains(e.target)) {
                const albumId = playButton.dataset.albumid;
                if (albumId) {
                    console.log('Playing album with ID:', albumId);
                    fetchAlbumTracks(albumId).then(tracks => {
                        trackList = tracks.map(track => ({
                            file: track.audioUrl || track.file || '',
                            title: track.name || track.title || 'Unnamed Track',
                            artist: track.userName || track.artist || track.creator || 'Unknown Artist',
                            image: track.imageLink || track.image || track.coverImage || ''
                        })).filter(track => track.file && track.file.trim().length > 0);
                        console.log('Mapped trackList for album:', trackList);
                        if (trackList.length > 0) {
                            currentTrackIndex = 0;
                            loadTrack(0, true);
                        } else {
                            console.log('No valid tracks in this album');
                        }
                    }).catch(error => console.error('Fetch error:', error));
                }
            } else {
                const albumId = albumCard.dataset.albumid;
                if (albumId) {
                    console.log('Showing grid for album with ID:', albumId);
                    showAlbumTracks(albumId);
                }
            }
        }

        // Handle playlist card clicks
        const playlistCard = e.target.closest('.library-track-card[data-type="playlist"]');
        if (playlistCard) {
            const playButton = playlistCard.querySelector('.playlist-play-btn');
            if (e.target === playButton || playButton.contains(e.target)) {
                const playlistId = playButton.dataset.playlistid;
                if (playlistId) {
                    console.log('Playing playlist with ID:', playlistId);
                    fetchPlaylistTracks(playlistId).then(tracks => {
                        trackList = tracks.map(track => ({
                            file: track.audioUrl || track.file || '',
                            title: track.name || track.title || 'Unnamed Track',
                            artist: track.userName || track.artist || track.creator || 'Unknown Artist',
                            image: track.imageLink || track.image || track.coverImage || ''
                        })).filter(track => track.file && track.file.trim().length > 0);
                        console.log('Mapped trackList for playlist:', trackList);
                        if (trackList.length > 0) {
                            currentTrackIndex = 0;
                            loadTrack(0, true);
                        } else {
                            console.log('No valid tracks in this playlist');
                        }
                    }).catch(error => console.error('Fetch error:', error));
                }
            } else {
                const playlistId = playlistCard.dataset.playlistid;
                if (playlistId) {
                    console.log('Showing grid for playlist with ID:', playlistId);
                    showPlaylistTracks(playlistId);
                }
            }
        }

        // Handle tracklist play button clicks (for both album and playlist grids)
        const trackPlayButton = e.target.closest('.tracklist-play-btn');
        if (trackPlayButton) {
            const index = parseInt(trackPlayButton.dataset.index);
            if (!isNaN(index) && currentTracks && currentTracks.length > index) {
                trackList = currentTracks.slice(index).map(track => ({
                    file: track.audioUrl || track.file || '',
                    title: track.name || track.title || 'Unnamed Track',
                    artist: track.userName || track.artist || track.creator || 'Unknown Artist',
                    image: track.imageLink || track.image || track.coverImage || ''
                })).filter(track => track.file && track.file.trim().length > 0);
                console.log('Mapped trackList from grid:', trackList);
                if (trackList.length > 0) {
                    currentTrackIndex = 0;
                    loadTrack(0, true);
                } else {
                    console.log('No valid tracks from this point');
                }
            } else {
                // Fallback to single track playback
                const file = trackPlayButton.dataset.src;
                const title = trackPlayButton.dataset.title || 'Unnamed Track';
                const artist = trackPlayButton.dataset.artist || 'Unknown Artist';
                const image = trackPlayButton.dataset.image || '';
                if (file && file !== 'undefined') {
                    trackList = [{ file, title, artist, image }];
                    currentTrackIndex = 0;
                    loadTrack(0, true);
                } else {
                    console.log('No valid audio URL for this track in grid:', file);
                }
            }
        }

        // Handle single track play button clicks
        const singlePlayButton = e.target.closest('.play-track-btn');
        if (singlePlayButton) {
            console.log('Single track button clicked:', singlePlayButton);
            const file = singlePlayButton.dataset.src;
            const title = singlePlayButton.dataset.title || 'Unnamed Track';
            const artist = singlePlayButton.dataset.artist || 'Unknown Artist';
            const image = singlePlayButton.dataset.image || '';
            if (file && file !== 'undefined') {
                trackList = [{ file, title, artist, image }];
                currentTrackIndex = 0;
                loadTrack(0, true);
            } else {
                console.log('No valid audio URL for this single track:', file);
            }
        }
    });

    // Fetch album tracks from the server
    async function fetchAlbumTracks(albumId) {
        try {
            const response = await fetch('/Album/Tracks?albumId=' + albumId);
            if (!response.ok) {
                throw new Error('Failed to fetch album tracks: Server returned ' + response.status);
            }
            const tracks = await response.json();
            console.log('Raw album tracks data:', tracks); // Log raw data for debugging
            return tracks;
        } catch (error) {
            console.error(error);
            alert('Error fetching album tracks. Please try again later.');
            return [];
        }
    }

    // Display album tracks in a grid
    async function showAlbumTracks(albumId) {
        selectedAlbumId = albumId;
        hideAlbumTracksGrid();
        const tracks = await fetchAlbumTracks(albumId);
        currentTracks = tracks;
        console.log('Tracks for grid display:', tracks);
        populateTracksGrid(tracks);
        document.getElementById('albums-content').style.display = 'none';
        document.getElementById('selected-album-tracks').style.display = 'grid';

        const backButton = document.getElementById('back-to-albums');
        if (!backButton) {
            const backButtonHtml = `<button id="back-to-albums" class="back-button">Back to Albums</button>`;
            document.getElementById('selected-album-tracks').insertAdjacentHTML('beforeend', backButtonHtml);
            document.getElementById('back-to-albums').addEventListener('click', hideAlbumTracksGrid);
        }
    }

    // Hide album tracks grid and return to albums view
    function hideAlbumTracksGrid() {
        selectedAlbumId = null;
        document.getElementById('albums-content').style.display = 'grid';
        document.getElementById('selected-album-tracks').style.display = 'none';
        const backButton = document.getElementById('back-to-albums');
        if (backButton) {
            backButton.remove();
        }
    }

    // Fetch playlist tracks from the server
    async function fetchPlaylistTracks(playlistId) {
        try {
            const response = await fetch('/Playlist/Tracks?playlistId=' + playlistId);
            if (!response.ok) {
                throw new Error('Failed to fetch playlist tracks: Server returned ' + response.status);
            }
            const tracks = await response.json();
            console.log('Raw playlist tracks data:', tracks); // Log raw data for debugging
            return tracks;
        } catch (error) {
            console.error(error);
            alert('Error fetching playlist tracks. Please try again later.');
            return [];
        }
    }

    // Display playlist tracks in a grid
    async function showPlaylistTracks(playlistId) {
        selectedPlaylistId = playlistId;
        hidePlaylistTracksGrid();
        const tracks = await fetchPlaylistTracks(playlistId);
        currentTracks = tracks;
        console.log('Tracks for playlist grid display:', tracks);
        populateTracksGrid(tracks, 'selected-playlist-tracks');
        document.getElementById('playlists-content').style.display = 'none';
        document.getElementById('selected-playlist-tracks').style.display = 'grid';

        const backButton = document.getElementById('back-to-playlists');
        if (!backButton) {
            const backButtonHtml = `<button id="back-to-playlists" class="back-button">Back to Playlists</button>`;
            document.getElementById('selected-playlist-tracks').insertAdjacentHTML('beforeend', backButtonHtml);
            document.getElementById('back-to-playlists').addEventListener('click', hidePlaylistTracksGrid);
        }
    }

    // Hide playlist tracks grid and return to playlists view
    function hidePlaylistTracksGrid() {
        selectedPlaylistId = null;
        document.getElementById('playlists-content').style.display = 'grid';
        document.getElementById('selected-playlist-tracks').style.display = 'none';
        const backButton = document.getElementById('back-to-playlists');
        if (backButton) {
            backButton.remove();
        }
    }

    // Populate the tracks grid with track data
    function populateTracksGrid(tracks, containerId = 'selected-album-tracks') {
        const grid = document.getElementById(containerId);
        grid.innerHTML = '';
        grid.className = 'tracklist-grid';
        if (!tracks || tracks.length === 0) {
            grid.innerHTML = '<p class="no-tracks">No tracks in this playlist.</p>';
        } else {
            tracks.forEach((track, index) => {
                const rowHtml = `
                    <div class="tracklist-row">
                        <div class="tracklist-cell image">
                            <img src="${track.imageLink || track.image || track.coverImage || ''}" alt="Track Image" class="tracklist-image">
                        </div>
                        <div class="tracklist-cell title">${track.name || track.title || 'Unnamed Track'}</div>
                        <div class="tracklist-cell artist">${track.userName || track.artist || track.creator || 'Unknown Artist'}</div>
                        <div class="tracklist-cell actions">
                            <button class="tracklist-play-btn" 
                                    data-index="${index}"
                                    data-src="${track.audioUrl || track.file || ''}" 
                                    data-title="${track.name || track.title || ''}" 
                                    data-artist="${track.userName || track.artist || track.creator || ''}" 
                                    data-image="${track.imageLink || track.image || track.coverImage || ''}">
                                <i class="fas fa-play"></i>
                            </button>
                        </div>
                    </div>`;
                grid.innerHTML += rowHtml;
            });
        }
    }
});