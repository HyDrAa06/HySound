document.addEventListener("DOMContentLoaded", function () {
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
    const soundBar = document.getElementById("sound-bar");

    const navLinks = document.getElementsByClassName("nav-link");

    let trackList = [];
    let currentTrackIndex = 0;
    let isPlaying = false;
    let selectedAlbumId = null;

    function updateProgressBar(bar, value) {
        bar.style.background = `linear-gradient(to right, #1db954 0%, #1db954 ${value}%, #1e1e1e ${value}%, #1e1e1e 100%)`;
    }

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

    for (let navLink of navLinks) {
        navLink.addEventListener("click", function () {
            saveTrackState();
        });
    }

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

    prevButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex - 1 + trackList.length) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

    nextButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex + 1) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

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

    globalAudio.addEventListener("timeupdate", function () {
        if (!isNaN(globalAudio.duration)) {
            const progress = (globalAudio.currentTime / globalAudio.duration) * 100;
            timebar.value = progress || 0;
            updateProgressBar(timebar, timebar.value);
        }
    });

    timebar.addEventListener("input", function () {
        globalAudio.currentTime = (timebar.value / 100) * globalAudio.duration;
        updateProgressBar(timebar, timebar.value);
        saveTrackState();
    });

    soundBar.addEventListener("input", function () {
        globalAudio.volume = soundBar.value / 100;
        updateProgressBar(soundBar, soundBar.value);
    });

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
        document.getElementById('selected-album-tracks').style.display = 'none';
        if (content === 'favourites') {
            document.getElementById('favourites-content').style.display = 'grid';
        } else if (content === 'playlists') {
            document.getElementById('playlists-content').style.display = 'grid';
        } else if (content === 'albums') {
            if (selectedAlbumId) {
                showAlbumTracks(selectedAlbumId);
            } else {
                document.getElementById('albums-content').style.display = 'grid';
            }
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
                            file: track.audioUrl || '',
                            title: track.name || 'Unnamed Track',
                            artist: track.userName || 'Unknown Artist',
                            image: track.imageLink || ''
                        })).filter(track => track.file && track.file.trim().length > 0);
                        if (trackList.length > 0) {
                            loadTrack(0, true);
                        } else {
                            console.log('No valid tracks in this album');
                        }
                    }).catch(error => console.error('Fetch error:', error));
                } else {
                    console.error('No albumId found for this play button');
                }
            } else {
                const albumId = albumCard.dataset.albumid;
                if (albumId) {
                    console.log('Showing grid for album with ID:', albumId);
                    showAlbumTracks(albumId);
                } else {
                    console.error('No albumId found for this album card');
                }
            }
        }

        // Handle tracklist play button clicks
        const trackPlayButton = e.target.closest('.tracklist-play-btn');
        if (trackPlayButton) {
            console.log('Track button clicked in grid:', trackPlayButton);
            const file = trackPlayButton.dataset.src;
            const title = trackPlayButton.dataset.title;
            const artist = trackPlayButton.dataset.artist;
            const image = trackPlayButton.dataset.image;
            if (file && file !== 'undefined') {
                trackList = [{ file, title, artist, image }];
                loadTrack(0, true);
            } else {
                console.log('No valid audio URL for this track in grid:', file);
            }
        }

        // Handle single track play button clicks (no style change)
        const singlePlayButton = e.target.closest('.play-track-btn');
        if (singlePlayButton) {
            console.log('Single track button clicked:', singlePlayButton);
            const file = singlePlayButton.dataset.src;
            const title = singlePlayButton.dataset.title;
            const artist = singlePlayButton.dataset.artist;
            const image = singlePlayButton.dataset.image;
            if (file && file !== 'undefined') {
                trackList = [{ file, title, artist, image }];
                loadTrack(0, true);
            } else {
                console.log('No valid audio URL for this single track:', file);
            }
        }
    });

    async function fetchAlbumTracks(albumId) {
        try {
            const response = await fetch('/Album/Tracks?albumId=' + albumId);
            if (!response.ok) {
                throw new Error('Failed to fetch album tracks: Server returned ' + response.status);
            }
            return await response.json();
        } catch (error) {
            console.error(error);
            alert('Error fetching album tracks. Please try again later.');
            return [];
        }
    }

    async function showAlbumTracks(albumId) {
        selectedAlbumId = albumId;
        hideAlbumTracksGrid();
        const tracks = await fetchAlbumTracks(albumId);
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

    function hideAlbumTracksGrid() {
        selectedAlbumId = null;
        document.getElementById('albums-content').style.display = 'grid';
        document.getElementById('selected-album-tracks').style.display = 'none';
        const backButton = document.getElementById('back-to-albums');
        if (backButton) {
            backButton.remove();
        }
    }

    function populateTracksGrid(tracks) {
        const grid = document.getElementById('selected-album-tracks');
        grid.innerHTML = '';
        grid.className = 'tracklist-grid'; // Ensure the grid uses the new class
        if (!tracks || tracks.length === 0) {
            grid.innerHTML = '<p class="no-tracks">No tracks in this album.</p>';
        } else {
            tracks.forEach(track => {
                const rowHtml = `
                    <div class="tracklist-row">
                        <div class="tracklist-cell image">
                            <img src="${track.imageLink || ''}" alt="Track Image" class="tracklist-image">
                        </div>
                        <div class="tracklist-cell title">${track.name || 'Unnamed Track'}</div>
                        <div class="tracklist-cell artist">${track.userName || 'Unknown Artist'}</div>
                        <div class="tracklist-cell actions">
                            <button class="tracklist-play-btn" 
                                    data-src="${track.audioUrl || ''}" 
                                    data-title="${track.name || ''}" 
                                    data-artist="${track.userName || ''}" 
                                    data-image="${track.imageLink || ''}">
                                <i class="fas fa-play"></i>
                            </button>
                        </div>
                    </div>`;
                grid.innerHTML += rowHtml;
            });
        }
    }
});